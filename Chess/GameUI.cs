using System;
using System.Collections;
using System.Windows.Forms;
using ChessLibrary;

namespace Chess
{
	
	public class GameUI
	{
		private ArrayList Squars;	
		public Images ChessImages;	
		private string ResourceFolder;		
		private int LogCounter;			

		public Game ChessGame;		   
		public Sounds	Sounds;			
		public string	SelectedSquar;	
        public string LastSelectedSquar;
		public ChessMain ParentForm;	
		public bool ShowMoveHelp;		
		public bool IsRunning;			
		public bool IsOver;				
        public bool ShowComputerThinkingProgres = true;    
        public bool LastMoveByClick;    

		public GameUI(ChessMain form)
		{
			this.ParentForm = form;	

			
			ChessImages = new Images();

            #if DEBUG
			    ResourceFolder = "..\\..\\Resources\\";
            #else
                ResourceFolder = "Resources\\";
            #endif

            
            ResourceFolder = "Resources\\";
			ChessImages.LoadImages(ResourceFolder);
			Sounds = new Sounds(ResourceFolder);	
			BuildBoard();
			
			ParentForm.ChessCaptureBar.InitializeBar(ChessImages);	

			
			ShowMoveHelp = true; 
		}

		
		public void BuildBoard()
		{
			Squars = new ArrayList();	

			
			for (int row=1; row<=8; row++)		
				for (int col=1; col<=8; col++)	
				{
					Squar ChessSquar = new Squar(row, col, this);
					ChessSquar.SetBackgroundSquar(ChessImages);	
					Squars.Add(ChessSquar);
					ParentForm.Controls.Add(ChessSquar);
				}
		}

	
		private Squar GetBoardSquar(string strCellName)
		{
			foreach (Squar ChessSquar in Squars)
			{
				if (ChessSquar.Name == strCellName)
					return ChessSquar;
			}
			return null;
		}

		
		public void RedrawBoard()
		{
			foreach (Squar ChessSquar in Squars)
			{
				if (ChessSquar.BackgroundImage==null) 
				{
					ChessSquar.SetBackgroundSquar(ChessImages);
				}

				if (ChessGame.Board[ChessSquar.Name] != null)	
					ChessSquar.DrawPiece(ChessImages.GetImageForPiece(ChessGame.Board[ChessSquar.Name].piece )); 
				
				if (ChessSquar.Name == SelectedSquar && ShowMoveHelp==true) 
				{
					ChessSquar.BackgroundImage = null;
					ChessSquar.BackColor = System.Drawing.Color.Thistle;
				}
			}

			
			if (SelectedSquar != null && SelectedSquar != "" && ShowMoveHelp==true && ChessGame.Board[SelectedSquar].piece != null && !ChessGame.Board[SelectedSquar].piece.IsEmpty() &&  ChessGame.Board[SelectedSquar].piece.Side.type == ChessGame.GameTurn )
			{
				ArrayList moves=ChessGame.GetLegalMoves(ChessGame.Board[SelectedSquar]);	
			
				
				foreach (Cell cell in moves)
				{
					Squar sqr=GetBoardSquar(cell.ToString());	
					sqr.BackgroundImage = null;
                    
                    sqr.BackColor = System.Drawing.Color.FromArgb(200, System.Drawing.Color.SaddleBrown);
				}
			}
			SelectedSquar="";	
		}

		
		public void ShowPlayerTurn()
		{
			ChessGame.UpdateTime();	

			if (ChessGame.BlackTurn())
			{
				ParentForm.BlackPlayerTime.Text = ChessGame.BlackPlayer.ThinkTime;
				
			}
			else
			{
				ParentForm.WhitePlayerTime.Text = ChessGame.WhitePlayer.ThinkTime;
				
			}
		}

		
		public void NextPlayerTurn()
		{
			if (ChessGame.ActivePlay.IsComputer()) 
			{
                if (ShowComputerThinkingProgres)
                    ParentForm.ChessCaptureBar.Visible = false;
                else
                    ParentForm.ChessCaptureBar.Visible = true;

				Move nextMove = ChessGame.ActivePlay.GetBestMove();	

				if (nextMove!=null)	
					UserMove(nextMove.StartCell.ToString(), nextMove.EndCell.ToString());
				
				ParentForm.ChessCaptureBar.Visible = true; 
			}
		}

		
		private void InitPlayers()
		{
           
            if (ChessGame.BlackPlayer.PlayerType == Player.Type.Human && ChessGame.WhitePlayer.PlayerType == Player.Type.Human)
            {
                ChessGame.BlackPlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "user.jpg");
                ChessGame.WhitePlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "user_2.jpg");
            }
            else if (ChessGame.BlackPlayer.PlayerType == Player.Type.Computer && ChessGame.WhitePlayer.PlayerType == Player.Type.Human)
            {
                ChessGame.BlackPlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "laptop.jpg");
                ChessGame.WhitePlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "user_2.jpg");
            }
            else if (ChessGame.BlackPlayer.PlayerType == Player.Type.Computer && ChessGame.WhitePlayer.PlayerType == Player.Type.Computer)
            {
                ChessGame.BlackPlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "laptop.jpg");
                ChessGame.WhitePlayer.Image = System.Drawing.Image.FromFile(ResourceFolder + "laptop_2.png");
            }

			
			ParentForm.WhitePlayerName.Text = ChessGame.WhitePlayer.Name;
			ParentForm.BlackPlayerName.Text = ChessGame.WhitePlayer.Name;

			ParentForm.WhitePlayerImage.Image = ChessGame.WhitePlayer.Image;
			ParentForm.BlackPlayerImage.Image = ChessGame.BlackPlayer.Image;

			ParentForm.WhitePlayerName.Text = ChessGame.WhitePlayer.Name;
			ParentForm.BlackPlayerName.Text = ChessGame.BlackPlayer.Name;

			// Set the time 
			ParentForm.BlackPlayerTime.Text = "00:00:00";
			ParentForm.WhitePlayerTime.Text = "00:00:00";

			ParentForm.lstHistory.Items.Clear();
		}

		
		public bool UserMove(string source, string dest)
		{
            bool success = true;
			int MoveResult=ChessGame.DoMove(source, dest);
			RedrawBoard();	

			switch (MoveResult)
			{
				case 0:	
					Move move=ChessGame.GetLastMove();	

					
					if (ChessGame.IsUnderCheck())
						Sounds.PlayCheck();	
					else if (move.Type == Move.MoveType.NormalMove || move.Type == Move.MoveType.TowerMove)
						Sounds.PlayNormalMove();
					else
						Sounds.PlayCaptureMove();

					
					if ( move.IsCaptureMove() )
						ParentForm.ChessCaptureBar.Add(ChessImages.GetImageForPiece(move.CapturedPiece));

					
					if (move.IsPromoMove() && !ChessGame.ActivePlay.IsComputer())
						ChessGame.SetPromoPiece(GetPromoPiece(move.EndCell.piece.Side));	
					
					
					if (ChessGame.IsCheckMate(ChessGame.GameTurn))
					{
						Sounds.PlayGameOver();
						IsOver=true;
						MessageBox.Show(ChessGame.GetPlayerBySide(ChessGame.GameTurn).Name + " is checkmate.", "Game Over",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
					}
					
					if (ChessGame.IsStaleMate(ChessGame.GameTurn))
					{
						Sounds.PlayGameOver();
						IsOver=true;
						MessageBox.Show(ChessGame.GetPlayerBySide(ChessGame.GameTurn).Name + " is stalmate.", "Game Over",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
					}
					LogUserMove(move.ToString());	
					NextPlayerTurn();
					break;

				default:
                    success = false;
					break;
			}

            return success;
		}

		
		public Piece GetPromoPiece(Side PlayerSide)
		{
			SelectPiece SelectPieceDlg = new SelectPiece();

			
			SelectPieceDlg.Piece1.Image = ChessImages.GetImageForPiece(new Piece(Piece.PieceType.Queen,PlayerSide));
			SelectPieceDlg.Piece2.Image = ChessImages.GetImageForPiece(new Piece(Piece.PieceType.Knight,PlayerSide));
			SelectPieceDlg.Piece3.Image = ChessImages.GetImageForPiece(new Piece(Piece.PieceType.Rook,PlayerSide));
			SelectPieceDlg.Piece4.Image = ChessImages.GetImageForPiece(new Piece(Piece.PieceType.Bishop,PlayerSide));
			
			SelectPieceDlg.ShowDialog(this.ParentForm);	

			 
			switch (SelectPieceDlg.SelectedIndex)
			{
				case 1:
					return new Piece(Piece.PieceType.Queen,PlayerSide);
				case 2:
					return new Piece(Piece.PieceType.Knight,PlayerSide);
				case 3:
					return new Piece(Piece.PieceType.Rook,PlayerSide);
				case 4:
					return new Piece(Piece.PieceType.Bishop,PlayerSide);
			}
			return null;
		}

		
		public void LogUserMove(string movestring)
		{
			LogCounter++;
			ListViewItem newItem = new ListViewItem(new string[] { LogCounter.ToString(), movestring}, -1);
			
			if (LogCounter % 2 == 0)	
				newItem.ForeColor = System.Drawing.Color.Blue;

			ParentForm.lstHistory.Items.Add(newItem);
			ParentForm.lstHistory.Items[ParentForm.lstHistory.Items.Count-1].EnsureVisible();	
		
           
            if (ParentForm.lstHistory.Items.Count > 16)
                ParentForm.lstHistory.Columns[1].Width = 90;
        }

		
		public void UndoMove()
		{
			IsOver=false;				
			Sounds.PlayNormalMove();

            
            Move move = ChessGame.GetLastMove();	 

			if (ChessGame.UnDoMove())
			{
				LogUserMove("Undo Move");	

                
                if (move.IsCaptureMove())
                    ParentForm.ChessCaptureBar.RemoveLast();
			}

			
			if (ChessGame.ActivePlay.IsComputer())
			{
                move = ChessGame.GetLastMove();	
				ChessGame.UnDoMove();

                
                if (move.IsCaptureMove())
				    ParentForm.ChessCaptureBar.RemoveLast();
			}

			RedrawBoard();	
		}

		
		public void ComputerThinking(int depth, int currentMove, int TotalMoves, int TotalAnalzyed, Move BestMove)
		{
            if (ShowComputerThinkingProgres)
            {
                
                ParentForm.PrgComputerThinkDepth.Maximum = TotalMoves;
                ParentForm.PrgComputerThinkDepth.Value = currentMove;
                ParentForm.LblComuterThinkLabel.Text = "Computer thinking at depth " + depth.ToString() + ". Total moves analyzed: " + TotalAnalzyed + ". ";

                if (BestMove != null)
                    ParentForm.LblComuterThinkLabel.Text += "Best move found so far is :" + BestMove.ToString();
            }
		}

		
		public void RedoMove()
		{
			Sounds.PlayNormalMove();
			if (ChessGame.ReDoMove())
			{
				LogUserMove("Redo Move");	

				
				Move move=ChessGame.GetLastMove();	

				
				if ( move.IsCaptureMove() )
					ParentForm.ChessCaptureBar.Add(ChessImages.GetImageForPiece(move.CapturedPiece));
			}
			RedrawBoard();	
		}

        
        /// <param name="filePath"></param>
        public void SaveGame()
        {
           
            SaveFileDialog saveAsDialog = new SaveFileDialog();
            saveAsDialog.Title = "Save file as...";
            saveAsDialog.Filter = "CSChess File (*.qcf)|*.qcf";
            saveAsDialog.RestoreDirectory = true;

            if (saveAsDialog.ShowDialog() == DialogResult.OK)
            {
                
                ChessGame.SaveGame(saveAsDialog.FileName);
            }
        }

      
        /// <param name="filePath"></param>
        public void LoadGame()
        {
            
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Load CSChess file...";
            openDialog.Filter = "CSChess File (*.qcf)|*.qcf";
            openDialog.RestoreDirectory = true;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                ChessGame = new Game();
                ChessGame.Reset();	
                ParentForm.ChessCaptureBar.Clear();

                IsRunning = true;
                LogCounter = 0;

               
                ChessGame.ComputerThinking += new ChessLibrary.Game.ChessComputerThinking(ComputerThinking);

                
                ChessGame.LoadGame(openDialog.FileName);

                
                InitPlayers();
                ParentForm.BlackPlayerTime.Text = ChessGame.BlackPlayer.ThinkTime;
                ParentForm.WhitePlayerTime.Text = ChessGame.WhitePlayer.ThinkTime;

               
                object[] moves = ChessGame.MoveHistory.ToArray();
                for (int i = moves.Length - 1; i >= 0; i--)
                {
                    Move move = (Move)moves[i];

                    
                    LogUserMove(move.ToString());

                    
				    if ( move.IsCaptureMove() )
					    ParentForm.ChessCaptureBar.Add(ChessImages.GetImageForPiece(move.CapturedPiece));
                }

                
                ParentForm.EnableSaveMenu();
                ParentForm.SetGamePrefrencesMenu();

                RedrawBoard();		    
                NextPlayerTurn();		
            }
        }

		
		public void NewGame()
		{
			ParentForm.ChessCaptureBar.Clear();
			NewGame NewGameDlg = new NewGame();
            NewGameDlg.ResourceFolderPath = ResourceFolder;
			NewGameDlg.ShowDialog();

			
			if (NewGameDlg.bStartGame)
			{
				ChessGame = new Game();

				
				ChessGame.ComputerThinking += new ChessLibrary.Game.ChessComputerThinking(ComputerThinking);

				ChessGame.Reset();	
				IsRunning = true;
				LogCounter = 0;

				ChessGame.WhitePlayer.Name = NewGameDlg.WhitePlayerName.Text;
				ChessGame.BlackPlayer.Name = NewGameDlg.BlackPlayerName.Text;

				
                if (NewGameDlg.PlayersHvC.Checked)
                {
                    ChessGame.BlackPlayer.PlayerType = Player.Type.Computer;	
                    ChessGame.WhitePlayer.PlayerType = Player.Type.Human;	   
                }

				
				if (NewGameDlg.PlayersCvC.Checked)
				{
					ChessGame.BlackPlayer.PlayerType = Player.Type.Computer;	
					ChessGame.WhitePlayer.PlayerType = Player.Type.Computer;	
				}

				
				if (NewGameDlg.PlayerLevel1.Checked)
				{
					ChessGame.WhitePlayer.TotalThinkTime = 4;	
					ChessGame.BlackPlayer.TotalThinkTime = 4;	
				}

				
				if (NewGameDlg.PlayerLevel2.Checked)
				{
					ChessGame.WhitePlayer.TotalThinkTime = 8;	
					ChessGame.BlackPlayer.TotalThinkTime = 8;	
				}

				
				if (NewGameDlg.PlayerLevel3.Checked)
				{
					ChessGame.WhitePlayer.TotalThinkTime = 20;	
					ChessGame.BlackPlayer.TotalThinkTime = 20;	
				}

				InitPlayers();
				RedrawBoard();		
				NextPlayerTurn();		
			
                
                ParentForm.EnableSaveMenu();
            }
		}
	}
}

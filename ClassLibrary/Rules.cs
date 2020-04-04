using System;
using System.Collections;

namespace ChessLibrary
{
	
	public class Rules
	{
		private Board m_Board;	
		private Game m_Game;	

		public Rules(Board board, Game game)
		{
			m_Board=board;
			m_Game=game;
		}

		
		public Board ChessBoard
		{
			get 
			{
				return m_Board;	
			}
		}

		
		public Game ChessGame
		{
			get 
			{
				return m_Game;	
			}
		}

		
        public bool IsCheckMate(Side.SideType PlayerSide)
		{
			
			if ( IsUnderCheck(PlayerSide) && GetCountOfPossibleMoves(PlayerSide) == 0)
				return true;	
			else
				return false;
		}

		
        public bool IsStaleMate(Side.SideType PlayerSide)
		{
			
			if ( !IsUnderCheck(PlayerSide) && GetCountOfPossibleMoves(PlayerSide) == 0)
				return true;	
			else
				return false;
		}

		
		public int DoMove(Move move)
		{
			
            ArrayList LegalMoves = GetLegalMoves(m_Board[move.StartCell]);

            if (!LegalMoves.Contains(m_Board[move.EndCell]))	
				return -2;	

			
			SetMoveType(move);	
			ExecuteMove(move);	

			return 0;	
		}

		
		public void ExecuteMove(Move move)
		{
			
			switch (move.Type)
			{
				case Move.MoveType.CaputreMove:		
					DoNormalMove(move);
					break;

				case Move.MoveType.NormalMove:		
					DoNormalMove(move);
					break;

				case Move.MoveType.TowerMove:		
					DoTowerMove(move);
					break;

				case Move.MoveType.PromotionMove:	
					DoPromoMove(move);
					break;

				case Move.MoveType.EnPassant:		
					DoEnPassantMove(move);
					break;
			}
		}

		
		private void SetMoveType(Move move)
		{
			
			move.Type = Move.MoveType.NormalMove;

			
			if (move.EndCell.piece != null && move.EndCell.piece.Type != Piece.PieceType.Empty ) 
				move.Type = Move.MoveType.CaputreMove;

			
			if (move.StartCell.piece != null && move.StartCell.piece.Type == Piece.PieceType.King)
			{
				if (Math.Abs(move.EndCell.col - move.StartCell.col)>1)	
					move.Type = Move.MoveType.TowerMove;
			}

			
			if (move.StartCell.piece != null && move.StartCell.piece.Type == Piece.PieceType.Pawn)
			{
				
				if (move.EndCell.row == 8 || move.EndCell.row == 1)
					move.Type = Move.MoveType.PromotionMove;
			}		

			
			if (move.StartCell.piece != null && move.StartCell.piece.Type == Piece.PieceType.Pawn)
			{
				
				if ((move.EndCell.piece == null || move.EndCell.piece.IsEmpty()) && move.StartCell.col != move.EndCell.col )
					move.Type = Move.MoveType.EnPassant;
			}
		}

		
		private void DoNormalMove(Move move)
		{
			m_Board[move.StartCell].piece.Moves++;	
			m_Board[move.EndCell].piece = m_Board[move.StartCell].piece;		
			m_Board[move.StartCell].piece = new Piece(Piece.PieceType.Empty);	
		}

		
		private void DoTowerMove(Move move)
		{
			DoNormalMove(move);	

			
			if (move.EndCell.col > move.StartCell.col)
			{
				Cell rockcell = m_Board.RightCell(move.EndCell);
				Move newmove = new Move(rockcell,m_Board.LeftCell(move.EndCell)); 
				DoNormalMove(newmove); 
			}
			else
			{
				
				Cell rockcell = m_Board.LeftCell(move.EndCell);
                rockcell = m_Board.LeftCell(rockcell);
				Move newmove = new Move(rockcell,m_Board.RightCell(move.EndCell)); 
				DoNormalMove(newmove); 
			}
		}

		
		private void DoPromoMove(Move move)
		{
			DoNormalMove(move);	
			
			if (move.PromoPiece==null)
				m_Board[move.EndCell].piece = new Piece(Piece.PieceType.Queen, m_Board[move.EndCell].piece.Side);	
			else
				m_Board[move.EndCell].piece = move.PromoPiece;
		}

		
		private void DoEnPassantMove(Move move)
		{
			Cell EnPassantCell;

			if (move.StartCell.piece.Side.isWhite())	
				EnPassantCell = m_Board.BottomCell(move.EndCell);	
			else
				EnPassantCell = m_Board.TopCell(move.EndCell);	

			move.EnPassantPiece = EnPassantCell.piece;				
			EnPassantCell.piece = new Piece(Piece.PieceType.Empty);	
			DoNormalMove(move);	
		}

		
		public void UndoMove(Move move)
		{
			if (move.Type == Move.MoveType.CaputreMove || move.Type == Move.MoveType.NormalMove || move.Type == Move.MoveType.PromotionMove ) 
				UndoNormalMove(move);

			
			if (move.Type == Move.MoveType.TowerMove)
			{
				UndoNormalMove(move);	
				if (move.EndCell.col > move.StartCell.col) 
				{
					
					Cell source = m_Board.LeftCell(move.EndCell);	
					Cell target = m_Board[move.StartCell.row, 8];	

					m_Board[source].piece.Moves--;	
					m_Board[target].piece = m_Board[source].piece;		
					m_Board[source].piece = new Piece(Piece.PieceType.Empty);		
				}
				else	
				{
					
					Cell source = m_Board.RightCell(move.EndCell);	
					Cell target = m_Board[move.StartCell.row, 1];	

					m_Board[source].piece.Moves--;	
					m_Board[target].piece = m_Board[source].piece;		
					m_Board[source].piece = new Piece(Piece.PieceType.Empty);	
				}
			}

			
			if (move.Type == Move.MoveType.EnPassant)
			{
				Cell EnPassantCell;

				UndoNormalMove(move);
				if (move.StartCell.piece.Side.isWhite())	
					EnPassantCell = m_Board.BottomCell(move.EndCell);	
				else
					EnPassantCell = m_Board.TopCell(move.EndCell);	

				EnPassantCell.piece = move.EnPassantPiece;	
			}
		}

		
		private void UndoNormalMove(Move move)
		{
			m_Board[move.EndCell].piece = move.CapturedPiece;		
			m_Board[move.StartCell].piece = move.Piece;	
			m_Board[move.StartCell].piece.Moves--;	
		}

		
        public bool IsUnderCheck(Side.SideType PlayerSide)
		{
			Cell OwnerKingCell=null;
			ArrayList OwnerCells = m_Board.GetSideCell(PlayerSide);

			
			foreach (string CellName in OwnerCells)
			{
				if (m_Board[CellName].piece.Type == Piece.PieceType.King )
				{
					OwnerKingCell = m_Board[CellName]; 
					break;	
				}
			}

			
			ArrayList EnemyCells = m_Board.GetSideCell((new Side(PlayerSide)).Enemy());
			foreach (string CellName in EnemyCells)
			{
				ArrayList moves = GetPossibleMoves(m_Board[CellName]);	
				
				if (moves.Contains(OwnerKingCell))	
					return true;
			}
			return false;
		}

		
        private int GetCountOfPossibleMoves(Side.SideType PlayerSide)
		{
			int TotalMoves=0;
           
			
			ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide);
			foreach (string CellName in PlayerCells)
			{
				ArrayList moves = GetLegalMoves(m_Board[CellName]);	
				TotalMoves+=moves.Count;
			}
			return TotalMoves;
		}

		
		private bool CauseCheck(Move move)
		{
			bool CauseCheck=false;
            Side.SideType PlayerSide = move.StartCell.piece.Side.type;

			
			ExecuteMove(move);
			CauseCheck=IsUnderCheck(PlayerSide);
			UndoMove(move);	

			return CauseCheck;
		}

		
		public ArrayList GetLegalMoves(Cell source)
		{
			ArrayList LegalMoves;

			LegalMoves=GetPossibleMoves(source);	
			ArrayList ToRemove = new ArrayList();	

			
			foreach (Cell target in  LegalMoves)
			{
				
				if (CauseCheck(new Move(source, target)))	
					ToRemove.Add(target);
			}

            
            
            if (source.piece.Type == Piece.PieceType.King && IsUnderCheck(source.piece.Side.type))
            {
                foreach (Cell target in LegalMoves)
                {
                    
                    if (Math.Abs(target.col - source.col) > 1)
                        ToRemove.Add(target);
                }
            }

			
			foreach (Cell cell in  ToRemove)
			{
				LegalMoves.Remove(cell);	
			}
			return LegalMoves;
		}

		public ArrayList GenerateAllLegalMoves(Side PlayerSide)
		{
			ArrayList TotalMoves = new ArrayList();
			ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide.type);
			Move move;	

			
			foreach (string CellName in PlayerCells)
			{
				ArrayList moves = GetLegalMoves(m_Board[CellName]);	
				
				foreach (Cell dest in moves)
				{
					move = new Move(m_Board[CellName], dest);
					SetMoveType(move);				

					if (move.IsPromoMove())			
						move.Score=1000;
					else if (move.IsCaptureMove())	
						move.Score=move.EndCell.piece.GetWeight();	

					TotalMoves.Add(move);	
				}
			}

			
			MoveCompare moveCompareObj= new MoveCompare();
			TotalMoves.Sort(moveCompareObj);	

			return TotalMoves;
		}


		
		public ArrayList GenerateGoodCaptureMoves(Side PlayerSide)
		{
			ArrayList TotalMoves = new ArrayList();
			ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide.type);
			Move move;	

			
			foreach (string CellName in PlayerCells)
			{
				
				if (m_Board[CellName].piece.GetWeight()> 100)
				{
					ArrayList moves = GetLegalMoves(m_Board[CellName]);
				
					foreach (Cell dest in moves)
					{
						
						if ( dest.piece != null && !dest.piece.IsEmpty())
						{
							move = new Move(m_Board[CellName], dest);
							
							TotalMoves.Add(move);	
						}
					}
				}
			}
			return TotalMoves;
		}
		
		
		public ArrayList GetPossibleMoves(Cell source)
		{
			ArrayList LegalMoves = new ArrayList();

			
			switch (source.piece.Type)
			{
				case Piece.PieceType.Empty:	
					break;

				case Piece.PieceType.Pawn:	
					GetPawnMoves(source, LegalMoves);
					break;

				case Piece.PieceType.Knight:	
					GetKnightMoves(source, LegalMoves);
					break;

				case Piece.PieceType.Rook:	
					GetRookMoves(source, LegalMoves);
					break;

				case Piece.PieceType.Bishop:	
					GetBishopMoves(source, LegalMoves);
					break;

				case Piece.PieceType.Queen:	
					GetQueenMoves(source, LegalMoves);
					break;

				case Piece.PieceType.King:	
					GetKingMoves(source, LegalMoves);
					break;
			}

			return LegalMoves;
		}

		
		private Move LastMoveWasPawnBegin()
		{
			
			Move lastmove = m_Game.GetLastMove();

			if (lastmove!=null)	
			{
				if (lastmove.Piece.IsPawn()&& lastmove.Piece.Moves == 1)
				{
						return lastmove;
				}
			}
			return null;
		}

		
		private void GetPawnMoves(Cell source, ArrayList moves)
		{
			Cell newcell;

			if (source.piece.Side.isWhite())
			{
				
				newcell = m_Board.TopCell(source);	
				if (newcell!=null && newcell.IsEmpty()) 
					moves.Add(newcell);
				
				
				if (newcell != null && newcell.IsEmpty())
				{
					newcell = m_Board.TopCell(newcell);	
					if (newcell!=null && source.piece.Moves == 0 && newcell.IsEmpty()) 
						moves.Add(newcell);
				}

				
				newcell = m_Board.TopLeftCell(source);	
				if (newcell!=null && newcell.IsOwnedByEnemy(source)) 
					moves.Add(newcell);

				
				newcell = m_Board.TopRightCell(source);	
				if (newcell!=null && newcell.IsOwnedByEnemy(source)) 
					moves.Add(newcell);

				
				Move LastPawnMove=LastMoveWasPawnBegin();	

				if (LastPawnMove!=null)	
				{
					if (source.row == LastPawnMove.EndCell.row) 
					{
						if (LastPawnMove.EndCell.col == source.col-1)	
						{
							newcell = m_Board.TopLeftCell(source);	
							if (newcell!=null && newcell.IsEmpty()) 
								moves.Add(newcell);
						}

						if (LastPawnMove.EndCell.col == source.col+1)	
						{
							newcell = m_Board.TopRightCell(source);	
							if (newcell!=null && newcell.IsEmpty()) 
								moves.Add(newcell);
						}
					}
				}
			}
			else
			{
				
				newcell = m_Board.BottomCell(source);	
				if (newcell!=null && newcell.IsEmpty()) 
					moves.Add(newcell);
				
				
				if (newcell!=null && newcell.IsEmpty())
				{
					newcell = m_Board.BottomCell(newcell);	
					if (newcell!=null && source.piece.Moves == 0 && newcell.IsEmpty()) 
						moves.Add(newcell);
				}

				
				newcell = m_Board.BottomLeftCell(source);	
				if (newcell!=null && newcell.IsOwnedByEnemy(source)) 
					moves.Add(newcell);

				
				newcell = m_Board.BottomRightCell(source);	
				if (newcell!=null && newcell.IsOwnedByEnemy(source)) 
					moves.Add(newcell);

				
				Move LastPawnMove=LastMoveWasPawnBegin();	

				if (LastPawnMove!=null)	
				{
					if (source.row == LastPawnMove.EndCell.row) 
					{
						if (LastPawnMove.EndCell.col == source.col-1)	
						{
							newcell = m_Board.BottomLeftCell(source);	
							if (newcell!=null && newcell.IsEmpty()) 
								moves.Add(newcell);
						}

						if (LastPawnMove.EndCell.col == source.col+1)	
						{
							newcell = m_Board.BottomRightCell(source);	
							if (newcell!=null && newcell.IsEmpty()) 
								moves.Add(newcell);
						}
					}
				}
			}
		}

		
		private void GetKnightMoves(Cell source, ArrayList moves)
		{
			Cell newcell;

			
			newcell = m_Board.TopCell(source);
			if (newcell!=null)
			{
				newcell = m_Board.TopLeftCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source)) 
					moves.Add(newcell);

				newcell = m_Board.TopCell(source);
				newcell = m_Board.TopRightCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);
			}
			
			newcell = m_Board.BottomCell(source);
			if (newcell!=null)
			{
				newcell = m_Board.BottomLeftCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);

				newcell = m_Board.BottomCell(source);
				newcell = m_Board.BottomRightCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);
			}
			
			newcell = m_Board.LeftCell(source);
			if (newcell!=null)
			{
				newcell = m_Board.TopLeftCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);

				newcell = m_Board.LeftCell(source);
				newcell = m_Board.BottomLeftCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);
			}
			
			newcell = m_Board.RightCell(source);
			if (newcell!=null)
			{
				newcell = m_Board.TopRightCell(newcell);
			
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);

				newcell = m_Board.RightCell(source);
				newcell = m_Board.BottomRightCell(newcell);
				
				if (newcell!=null && !newcell.IsOwned(source) ) 
					moves.Add(newcell);
			}
		}

		
		private void GetRookMoves(Cell source, ArrayList moves)
		{
			Cell newcell;

			
			newcell = m_Board.TopCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.TopCell(newcell); 
			}

			
			newcell = m_Board.LeftCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.LeftCell(newcell); 
			}

			
			newcell = m_Board.RightCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.RightCell(newcell); 
			}

			
			newcell = m_Board.BottomCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.BottomCell(newcell); 
			}
		}

		
		private void GetBishopMoves(Cell source, ArrayList moves)
		{
			Cell newcell;

			
			newcell = m_Board.TopLeftCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.TopLeftCell(newcell); 
			}

			newcell = m_Board.TopRightCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.TopRightCell(newcell); 
			}

			
			newcell = m_Board.BottomLeftCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.BottomLeftCell(newcell); 
			}

			
			newcell = m_Board.BottomRightCell(source);
			while (newcell!=null)	
			{
				if (newcell.IsEmpty())	
					moves.Add(newcell);

				if (newcell.IsOwnedByEnemy(source))	
				{
					moves.Add(newcell);	
					break;	
				}

				if (newcell.IsOwned(source))	
					break;	

				newcell = m_Board.BottomRightCell(newcell); 
			}
		}

		
		private void GetQueenMoves(Cell source, ArrayList moves)
		{
			
			GetRookMoves(source, moves); 
			GetBishopMoves(source, moves); 
		}

		
		private void GetKingMoves(Cell source, ArrayList moves)
		{
			Cell newcell;

			
	
			newcell = m_Board.TopCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.LeftCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.RightCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.BottomCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.TopLeftCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.TopRightCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.BottomLeftCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);
			
			newcell = m_Board.BottomRightCell(source);
			if (newcell!=null && !newcell.IsOwned(source)) 
				moves.Add(newcell);

			
			if (m_Board[source].piece.Moves == 0)
			{
				Cell CastlingTarget=null;	

				
				newcell = m_Board.RightCell(source);
				if (newcell!=null && newcell.IsEmpty())	
				{
					if (!CauseCheck(new Move(source, newcell)))
					{
                        newcell = m_Board.RightCell(newcell);
						if (newcell!=null && newcell.IsEmpty())	
						{
							CastlingTarget = newcell;	
                            newcell = m_Board.RightCell(newcell);
							if (newcell!=null && !newcell.IsEmpty()  && newcell.piece.Moves==0)	
								moves.Add(CastlingTarget);	
						} 
					}
				}

				
				newcell = m_Board.LeftCell(source);
				if (newcell!=null && newcell.IsEmpty())	
				{
					if (!CauseCheck(new Move(source, newcell))) 
					{
                        newcell = m_Board.LeftCell(newcell);
						if (newcell!=null && newcell.IsEmpty())	
						{
							CastlingTarget = newcell;	
                            newcell = m_Board.LeftCell(newcell);
							if (newcell!=null && newcell.IsEmpty())	
							{
                                newcell = m_Board.LeftCell(newcell);
								if (newcell!=null && !newcell.IsEmpty() && newcell.piece.Moves==0)	
									moves.Add(CastlingTarget);	
							}
						}

					}
				}
			}
		}

		
        public int AnalyzeBoard(Side.SideType PlayerSide)
		{
			int Score=0;
			ArrayList OwnerCells = m_Board.GetSideCell(PlayerSide);
			
			
			foreach (string ChessCell in OwnerCells)
			{
				Score+=m_Board[ChessCell].piece.GetWeight();
			}

			
			return Score;
		}

		
		public int Evaluate(Side PlayerSide)
		{
			int Score=0;

			Score=AnalyzeBoard(PlayerSide.type)-AnalyzeBoard(PlayerSide.Enemy())-25;

			if (IsCheckMate(PlayerSide.Enemy()))	
				Score=1000000;

			return Score;
		}

	}
}

using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace ChessLibrary
{
	
    [Serializable]
	public class Game
	{
		
		public delegate void ChessComputerThinking(int depth, int currentMove, int TotalMoves, int TotalAnalzyed , Move BestMove);

		public event ChessComputerThinking ComputerThinking;	

		public Board Board;		            
        public Side.SideType GameTurn;		    

        private Stack m_MovesHistory;		
        private Stack m_RedoMovesHistory;	
		private Rules m_Rules;			    
		private Player m_WhitePlayer;	    
		private Player m_BlackPlayer;	    

		public bool DoNullMovePruning;		
		public bool DoPrincipleVariation;	
		public bool DoQuiescentSearch;	
		
		public Game()
		{
			Board = new Board();

			m_Rules = new Rules(Board, this);	
			m_MovesHistory = new Stack();
			m_RedoMovesHistory = new Stack();
            m_WhitePlayer = new Player(new Side(Side.SideType.White), Player.Type.Human, m_Rules);	
            m_BlackPlayer = new Player(new Side(Side.SideType.Black), Player.Type.Human, m_Rules);	
		}

		
		public void NotifyComputerThinking(int depth, int currentMove, int TotalMoves, int TotalAnalzyed, Move BestMove)
		{
			if (ComputerThinking!=null)	
				ComputerThinking(depth, currentMove, TotalMoves, TotalAnalzyed, BestMove);
		}

		
		public Cell this[int row, int col]
		{
			get
			{
				return Board[row, col];
			}
		}

		
		public Cell this[string strloc]
		{
			get
			{
				return Board[strloc];	
			}
		}

		
		public bool CompVsCompGame()
		{
			return (m_WhitePlayer.PlayerType == m_BlackPlayer.PlayerType);
		}

       
        /// <param name="filePath"></param>
        public void SaveGame(string filePath)
        {
            try
            {
                
                XmlDocument gameXmlDocument = new XmlDocument();
                XmlNode gameXml = XmlSerialize(gameXmlDocument);

                gameXmlDocument.AppendChild(gameXmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                gameXmlDocument.AppendChild(gameXml);

               
                gameXmlDocument.Save(filePath);
                return;
            }
            catch (Exception) { }
        }

        
        /// <param name="filePath"></param>
        public void LoadGame(string filePath)
        {
            try
            {
                
                XmlDocument gameXmlDocument = new XmlDocument();
                gameXmlDocument.Load(filePath);

                XmlNode gameNode = gameXmlDocument.FirstChild;
                if (gameNode.NodeType == XmlNodeType.XmlDeclaration)
                    gameNode = gameNode.NextSibling;

               
                XmlDeserialize(gameNode);
            }
            catch (Exception) { }
        }

        
        private string GetChecksum(string content)
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(content));
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }

        
        public XmlNode XmlSerialize(XmlDocument xmlDoc)
        {
            XmlElement xmlGame = xmlDoc.CreateElement("Game");

           
            xmlGame.AppendChild(XMLHelper.CreateNodeWithValue(xmlDoc, "DoNullMovePruning", DoNullMovePruning.ToString()));
            xmlGame.AppendChild(XMLHelper.CreateNodeWithValue(xmlDoc, "DoPrincipleVariation", DoPrincipleVariation.ToString()));
            xmlGame.AppendChild(XMLHelper.CreateNodeWithValue(xmlDoc, "DoQuiescentSearch", DoQuiescentSearch.ToString()));

            
            xmlGame.AppendChild(XMLHelper.CreateNodeWithValue(xmlDoc, "GameTurn", GameTurn.ToString()));

            
            xmlGame.AppendChild(Board.XmlSerialize(xmlDoc));

            
            xmlGame.AppendChild(XMLHelper.CreateNodeWithXmlValue(xmlDoc, "WhitePlayer", XMLHelper.XmlSerialize(typeof(Player), m_WhitePlayer)));
            xmlGame.AppendChild(XMLHelper.CreateNodeWithXmlValue(xmlDoc, "BlackPlayer", XMLHelper.XmlSerialize(typeof(Player), m_BlackPlayer)));

            object[] moves = m_MovesHistory.ToArray();

            
            string xml = "";
            for (int i = moves.Length - 1; i >= 0; i-- )
            {
                Move move = (Move)moves[i];
                xml += XMLHelper.XmlSerialize(typeof(Move), move);
            }
            xmlGame.AppendChild(XMLHelper.CreateNodeWithXmlValue(xmlDoc, "MovesHistory", xml));

           
            string checksum = GetChecksum(xmlGame.InnerXml);
            (xmlGame as XmlElement).SetAttribute("Checksum", checksum);
            (xmlGame as XmlElement).SetAttribute("Version", "1.2");

            
            return xmlGame;
        }

        
        public void XmlDeserialize(XmlNode xmlGame)
        {
           
            if (xmlGame.Attributes["Checksum"] == null)
                return;

            
            DoNullMovePruning = (XMLHelper.GetNodeText(xmlGame, "DoNullMovePruning") == "True");
            DoPrincipleVariation = (XMLHelper.GetNodeText(xmlGame, "DoPrincipleVariation") == "True");
            DoQuiescentSearch = (XMLHelper.GetNodeText(xmlGame, "DoQuiescentSearch") == "True");

           
            GameTurn = (XMLHelper.GetNodeText(xmlGame, "DoQuiescentSearch") == "Black") ? Side.SideType.Black : Side.SideType.White;

            
            XmlNode xmlBoard = XMLHelper.GetFirstNodeByName(xmlGame, "Board");
            Board.XmlDeserialize(xmlBoard);

           
            XmlNode xmlPlayer = XMLHelper.GetFirstNodeByName(xmlGame, "WhitePlayer");
            m_WhitePlayer = (Player)XMLHelper.XmlDeserialize(typeof(Player), xmlPlayer.InnerXml);
            m_WhitePlayer.GameRules = m_Rules;

            xmlPlayer = XMLHelper.GetFirstNodeByName(xmlGame, "BlackPlayer");
            m_BlackPlayer = (Player)XMLHelper.XmlDeserialize(typeof(Player), xmlPlayer.InnerXml);
            m_BlackPlayer.GameRules = m_Rules;

            
            XmlNode xmlMoves = XMLHelper.GetFirstNodeByName(xmlGame, "MovesHistory");
            foreach (XmlNode xmlMove in xmlMoves.ChildNodes)
            {
                Move move = (Move)XMLHelper.XmlDeserialize(typeof(Move), xmlMove.OuterXml);
                m_MovesHistory.Push(move);
            }
        }

		
		public void Reset()
		{
			m_MovesHistory.Clear();
			m_RedoMovesHistory.Clear();

			
			m_WhitePlayer.ResetTime();
			m_BlackPlayer.ResetTime();

            GameTurn = Side.SideType.White;	
			m_WhitePlayer.TimeStart();	
			Board.Init();	
		}

		
		public Player WhitePlayer
		{
			get
			{
				return m_WhitePlayer;
			}
		}

		
		public Player BlackPlayer
		{
			get
			{
				return m_BlackPlayer;
			}
		}

		
		public Player ActivePlay
		{
			get
			{
				if (BlackTurn())
					return m_BlackPlayer;
				else
					return m_WhitePlayer;
			}
		}

		
		public Player EnemyPlayer(Side Player)
		{
			if (Player.isBlack())
				return m_WhitePlayer;
			else
				return m_BlackPlayer;
		}

		
        public Player GetPlayerBySide(Side.SideType type)
		{
            if (type == Side.SideType.Black)
				return m_BlackPlayer;
			else
				return m_WhitePlayer;
		}

		
		public void UpdateTime()
		{
			if (BlackTurn())	
				m_BlackPlayer.UpdateTime();
			else
				m_WhitePlayer.UpdateTime();
		}

		
		public bool BlackTurn()
		{
            return (GameTurn == Side.SideType.Black);
		}

		
		public bool WhiteTurn()
		{
            return (GameTurn == Side.SideType.White);
		}

		
		public void NextPlayerTurn()
		{
            if (GameTurn == Side.SideType.White)
			{
				m_WhitePlayer.TimeEnd();		
				m_BlackPlayer.TimeStart();		
                GameTurn = Side.SideType.Black;		
			}
			else
			{
				m_BlackPlayer.TimeEnd();
				m_WhitePlayer.TimeStart();		
                GameTurn = Side.SideType.White;		
			}
		}

		
		public ArrayList GetLegalMoves(Cell source)
		{
			return m_Rules.GetLegalMoves(source);
		}

		
		public int DoMove(string source, string dest)
		{
			int MoveResult;

			
            if (this.Board[source].piece != null && this.Board[source].piece.Type != Piece.PieceType.Empty && this.Board[source].piece.Side.type == GameTurn)
			{
				Move UserMove = new Move(this.Board[source], this.Board[dest]);	
				MoveResult=m_Rules.DoMove(UserMove);

				
				if (MoveResult==0)
				{
					m_MovesHistory.Push(UserMove);
					NextPlayerTurn();
				}
			}
			else
				MoveResult=-1;
			return MoveResult;	
		}

		
		public bool UnDoMove()
		{
			
			if (m_MovesHistory.Count>0)
			{
				Move UserMove = (Move)m_MovesHistory.Pop();	
				m_RedoMovesHistory.Push(UserMove);			
				m_Rules.UndoMove(UserMove);					
				NextPlayerTurn();							
				return true;
			}
			else
				return false;
		}

		
		public bool ReDoMove()
		{
			
			if (m_RedoMovesHistory.Count>0)
			{
				Move UserMove = (Move)m_RedoMovesHistory.Pop();	
				m_MovesHistory.Push(UserMove);				
				m_Rules.DoMove(UserMove);					
				NextPlayerTurn();							
				return true;
			}
			else
				return false;
		}

        
        public Stack MoveHistory
        {
            get { return m_MovesHistory; }
        }

		
        public bool IsCheckMate(Side.SideType PlayerSide)
		{
			return m_Rules.IsCheckMate(PlayerSide);
		}

		
        public bool IsStaleMate(Side.SideType PlayerSide)
		{
			return m_Rules.IsStaleMate(PlayerSide);
		}

		
		public bool IsUnderCheck()
		{
			return m_Rules.IsUnderCheck(GameTurn);
		}
		 

		
		public Move GetLastMove()
		{
			
			if (m_MovesHistory.Count>0)
			{
				return (Move)m_MovesHistory.Peek();	
			}
			return null;
		}

		
		public void SetPromoPiece(Piece PromoPiece)
		{
			
			if (m_MovesHistory.Count>0)
			{
				Move move=(Move)m_MovesHistory.Peek();	
				move.EndCell.piece = PromoPiece;	
				move.PromoPiece = PromoPiece;		
			}
		}
	}
}

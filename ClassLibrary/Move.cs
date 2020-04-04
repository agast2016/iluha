using System;

namespace ChessLibrary
{
	
    [Serializable]
	public class Move
	{
		public enum MoveType {NormalMove, CaputreMove, TowerMove, PromotionMove, EnPassant};	

		private Cell m_StartCell;	
		private Cell m_EndCell;		
		private Piece m_Piece;			
		private Piece m_CapturedPiece;	
		private Piece m_PromoPiece;		
		private Piece m_EnPassantPiece;	
		private MoveType m_Type;		
		private bool m_CauseCheck;		
		private int	m_Score;			

       
        internal Move()
        {
            m_Score = 0;
        }

		public Move(Cell begin, Cell end)
		{
			m_StartCell=begin;
			m_EndCell=end;
			m_Piece=begin.piece;
			m_CapturedPiece=end.piece;
			m_Score=0;
		}

		
		public Cell StartCell
		{
			get
			{
				return m_StartCell;
			}
            set
            {
                m_StartCell = value;
            }
		}

		
		public Cell EndCell
		{
			get
			{
				return m_EndCell;
			}
            set
            {
                m_EndCell = value;
            }
		}

		
		public Piece Piece
		{
			get
			{
				return m_Piece;
			}
            set
            {
                m_Piece = value;
            }
		}

	
		public Piece CapturedPiece
		{
			get
			{
				return m_CapturedPiece;
			}
            set
            {
                m_CapturedPiece = value;
            }
		}

		
		public MoveType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type=value;
			}
		}

		
		public bool CauseCheck
		{
			get
			{
				return m_CauseCheck;
			}
			set
			{
				m_CauseCheck=value;
			}
		}

		
		public Piece PromoPiece
		{
			get
			{
				return m_PromoPiece;
			}
			set
			{
				m_PromoPiece=value;
			}
		}

		
		public Piece EnPassantPiece
		{
			get
			{
				return m_EnPassantPiece;
			}
			set
			{
				m_EnPassantPiece=value;
			}
		}

		
		public int Score
		{
			get
			{
				return m_Score;
			}
			set
			{
				m_Score=value;
			}
		}

		
		public bool IsPromoMove()
		{
			return m_Type==MoveType.PromotionMove;
		}

		
		public bool IsCaptureMove()
		{
			return m_Type==MoveType.CaputreMove;
		}

		
		public override string ToString()
		{
			if (m_Type == Move.MoveType.CaputreMove)	
				return m_Piece + " " + m_StartCell.ToString2() + "x" + m_EndCell.ToString2();
			else
				return m_Piece + " " + m_StartCell.ToString2() + "-" + m_EndCell.ToString2();
		}
	}

	
	public class MoveCompare : System.Collections.IComparer
	{
		
		public MoveCompare()
		{
		}

		public int Compare(Object firstObj, Object SecondObj)
		{
			Move firstMove = (Move)firstObj;
			Move secondMove = (Move)SecondObj;

			return -firstMove.Score.CompareTo(secondMove.Score);
		}
	}
}

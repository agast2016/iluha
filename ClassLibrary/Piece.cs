using System;

namespace ChessLibrary 
{
	
    [Serializable]
	public class Piece
	{
		
		int m_moves;			
		Side m_side;			
		PieceType m_type;		

		public enum PieceType {Empty, King, Queen, Rook, Bishop, Knight, Pawn};	

	
		public Piece()
		{
			this.Type = PieceType.Empty;	
		}

		
		public Piece(PieceType type)
		{
			this.m_type = type;
		}

		
		public Piece(PieceType type, Side side)
		{
			this.m_type = type;
			this.m_side = side;
		}

		
		public bool IsEmpty()
		{
			return m_type==PieceType.Empty;
		}

		
		public bool IsPawn()
		{
			return m_type==PieceType.Pawn;
		}

		
		public bool IsKnight()
		{
			return m_type==PieceType.Knight;
		}

		
		public bool IsBishop()
		{
			return m_type==PieceType.Bishop;
		}

		
		public bool IsRook()
		{
			return m_type==PieceType.Rook;
		}

		
		public bool IsQueen()
		{
			return m_type==PieceType.Queen;
		}

		
		public bool IsKing()
		{
			return m_type==PieceType.King;
		}

		
		public override string ToString()
		{
			switch (m_type)
			{
				case PieceType.King:
					return "King";
				case PieceType.Queen:
					return "Queen";
				case PieceType.Bishop:
					return "Bishop";
				case PieceType.Rook:
					return "Rook";
				case PieceType.Knight:
					return "Knight";
				case PieceType.Pawn:
					return "Pawn";
				default:
					return "E";
			}
		}

		
		public int GetWeight()
		{
			switch (m_type)
			{
				case PieceType.King:
					return 0;
				case PieceType.Queen:
					return 900;
				case PieceType.Rook:
					return 500;
				case PieceType.Bishop:
					return 325;
				case PieceType.Knight:
					return 300;
				case PieceType.Pawn:
					return 100;
				default:
					return 0;
			}
		}

		#region Class attributes set and get methods
		
		public PieceType Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type=value;
			}
		}

		
		public Side Side
		{
			get
			{
				return m_side;
			}
			set
			{
				m_side=value;
			}
		}

		
		public int Moves
		{
			get
			{
				return m_moves;
			}
			set
			{
				m_moves=value;
			}
		}
		#endregion
	}
}

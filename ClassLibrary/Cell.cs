using System;

namespace ChessLibrary
{
	
    [Serializable]
	public class Cell
	{
		Piece m_piece;
		int m_row;
		int m_col;

		
		public Cell()
		{
			m_row=0;
			m_col=0;
		}

		
		public Cell(int irow, int icol)
		{
			m_row=irow;
			m_col=icol;
		}

		
		public Cell(string strLoc)
		{
			if(strLoc.Length==2)	
			{
				m_col=char.Parse(strLoc.Substring(0,1).ToUpper())-64; 
				m_row=int.Parse(strLoc.Substring(1,1));				  
			}
		}

		
		public bool IsDark
		{
			get
			{
				return ((row+col)%2==0);	
			}
		}

		
		public override string ToString()
		{
			string strLoc="";
			strLoc=Convert.ToString(Convert.ToChar(col+64));	
			strLoc+=row.ToString();								
			return strLoc;	
		}

		
		public string ToString2()
		{
			string strLoc="";
			int BoardRow = Math.Abs(8-row)+1;		
			strLoc=Convert.ToString(Convert.ToChar(col+64));	
			strLoc+=BoardRow.ToString();								
			return strLoc;	
		}

		
		public override bool Equals(object obj)
		{
			if (obj is Cell)
			{
				Cell cellObj=(Cell)obj;
				
				return (cellObj.row==row && cellObj.col==col);			   
			}
			return false;
		}

		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region Class attributes set and get methods
		
		public int row
		{
			get
			{
				return m_row;
			}
			set
			{
				m_row=value;
			}
		}

		
		public bool IsEmpty()
		{
			return m_piece == null || m_piece.Type == Piece.PieceType.Empty;
		}

		
		public bool IsOwnedByEnemy(Cell other)
		{
			if (IsEmpty())
				return false;
			else
                return m_piece.Side.type != other.piece.Side.type;
		}

		
		public bool IsOwned(Cell other)
		{
			if (IsEmpty())
				return false;
			else
				return m_piece.Side.type == other.piece.Side.type;
		}

		
		public int col
		{
			get
			{
				return m_col;
			}
			set
			{
				m_col=value;
			}
		}

		
		public Piece piece
		{
			get
			{
				return m_piece;
			}
			set
			{
				m_piece=value;
			}
		}
		#endregion

		
	}
}

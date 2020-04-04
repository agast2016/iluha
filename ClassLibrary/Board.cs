using System;
using System.Collections;
using System.Xml;

namespace ChessLibrary
{
	
    [Serializable]
	public class Board
	{
		private Side m_WhiteSide, m_BlackSide;	 
		private Cells m_cells;	

		public Board()
		{
            m_WhiteSide = new Side(Side.SideType.White);	
            m_BlackSide = new Side(Side.SideType.Black);	

			m_cells = new Cells();					
		}

	
		public void Init()
		{
			m_cells.Clear();		

			
			for (int row=1; row<=8; row++)
				for (int col=1; col<=8; col++)
				{
					m_cells.Add(new Cell(row,col));	
				}

			
			m_cells["a1"].piece = new Piece(Piece.PieceType.Rook,m_BlackSide);
			m_cells["h1"].piece = new Piece(Piece.PieceType.Rook,m_BlackSide);
			m_cells["b1"].piece = new Piece(Piece.PieceType.Knight,m_BlackSide);
			m_cells["g1"].piece = new Piece(Piece.PieceType.Knight,m_BlackSide);
			m_cells["c1"].piece = new Piece(Piece.PieceType.Bishop,m_BlackSide);
			m_cells["f1"].piece = new Piece(Piece.PieceType.Bishop,m_BlackSide);
			m_cells["e1"].piece = new Piece(Piece.PieceType.King,m_BlackSide);
			m_cells["d1"].piece = new Piece(Piece.PieceType.Queen,m_BlackSide);
			for (int col=1; col<=8; col++)
				m_cells[2, col].piece = new Piece(Piece.PieceType.Pawn,m_BlackSide);

			
			m_cells["a8"].piece = new Piece(Piece.PieceType.Rook,m_WhiteSide);
			m_cells["h8"].piece = new Piece(Piece.PieceType.Rook,m_WhiteSide);
			m_cells["b8"].piece = new Piece(Piece.PieceType.Knight,m_WhiteSide);
			m_cells["g8"].piece = new Piece(Piece.PieceType.Knight,m_WhiteSide);
			m_cells["c8"].piece = new Piece(Piece.PieceType.Bishop,m_WhiteSide);
			m_cells["f8"].piece = new Piece(Piece.PieceType.Bishop,m_WhiteSide);
			m_cells["e8"].piece = new Piece(Piece.PieceType.King,m_WhiteSide);
			m_cells["d8"].piece = new Piece(Piece.PieceType.Queen,m_WhiteSide);
			for (int col=1; col<=8; col++)
				m_cells[7, col].piece = new Piece(Piece.PieceType.Pawn,m_WhiteSide);
		}

		
		public Cell this[int row, int col]
		{
			get
			{
				return m_cells[row, col];
			}
		}

		
		public Cell this[string strloc]
		{
			get
			{
				return m_cells[strloc];	
			}
		}

		
		public Cell this[Cell cellobj]
		{
			get
			{
				return m_cells[cellobj.ToString()];	
			}
		}

        
        public XmlNode XmlSerialize(XmlDocument xmlDoc)
        {
            XmlElement xmlBoard = xmlDoc.CreateElement("Board");

            
            xmlBoard.AppendChild(m_WhiteSide.XmlSerialize(xmlDoc));
            xmlBoard.AppendChild(m_BlackSide.XmlSerialize(xmlDoc));

            xmlBoard.AppendChild(m_cells.XmlSerialize(xmlDoc));

            
            return xmlBoard;
        }

        
        public void XmlDeserialize(XmlNode xmlBoard)
        {
            
            XmlNode side = XMLHelper.GetFirstNodeByName(xmlBoard, "Side");

            
            m_WhiteSide.XmlDeserialize(side);
            m_BlackSide.XmlDeserialize(side.NextSibling);

            
            XmlNode xmlCells = XMLHelper.GetFirstNodeByName(xmlBoard, "Cells");
            m_cells.XmlDeserialize(xmlCells);
        }

		
		public ArrayList GetAllCells()
		{
			ArrayList CellNames = new ArrayList();

			
			for (int row=1; row<=8; row++)
				for (int col=1; col<=8; col++)
				{
					CellNames.Add(this[row,col].ToString()); 
				}

			return CellNames;
		}

		
        public ArrayList GetSideCell(Side.SideType PlayerSide)
		{
			ArrayList CellNames = new ArrayList();

			
			for (int row=1; row<=8; row++)
				for (int col=1; col<=8; col++)
				{
					
					if (this[row,col].piece!=null && !this[row,col].IsEmpty() && this[row,col].piece.Side.type == PlayerSide)
						CellNames.Add(this[row,col].ToString()); 
				}

			return CellNames;
		}

		
		public Cell TopCell(Cell cell)
		{
			return this[cell.row-1, cell.col];
		}

		
		public Cell LeftCell(Cell cell)
		{
			return this[cell.row, cell.col-1];
		}

		
		public Cell RightCell(Cell cell)
		{
			return this[cell.row, cell.col+1];
		}

		
		public Cell BottomCell(Cell cell)
		{
			return this[cell.row+1, cell.col];
		}

		
		public Cell TopLeftCell(Cell cell)
		{
			return this[cell.row-1, cell.col-1];
		}

		
		public Cell TopRightCell(Cell cell)
		{
			return this[cell.row-1, cell.col+1];
		}

		
		public Cell BottomLeftCell(Cell cell)
		{
			return this[cell.row+1, cell.col-1];
		}

		
		public Cell BottomRightCell(Cell cell)
		{
			return this[cell.row+1, cell.col+1];
		}
	}
}

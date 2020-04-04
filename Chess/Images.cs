using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using ChessLibrary;

namespace Chess
{
	
	public class Images
	{
		private ArrayList m_ImageList;		

		public Images()
		{
			m_ImageList = new ArrayList();
		}

		public void LoadImages(string SourceDir)
		{

			
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"Black.jpg"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"White.jpg"));
			
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"king.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"queen.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"bishop.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"knight.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"rook.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"pawn.gif"));
			
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"king_2.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"queen_2.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"bishop_2.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"knight_2.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"rook_2.gif"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"pawn_2.gif"));
			
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"Black_2.jpg"));
			m_ImageList.Add(System.Drawing.Image.FromFile(SourceDir+"White_2.jpg"));
		}

		
		public Image this[string strName]
		{
			get 
			{
				switch (strName)	
				{
					case "White":
						return (Image)m_ImageList[0];
					case "Black":
						return (Image)m_ImageList[1];
					case "White2":
						return (Image)m_ImageList[14];
					case "Black2":
						return (Image)m_ImageList[15];
					default:
						return null;

				}
				
			}
		}

		public Image GetImageForPiece(Piece Piece)
		{
			
			if (Piece == null || Piece.Type == Piece.PieceType.Empty )
				return null;

			if (Piece.Side.isWhite())
				switch(Piece.Type)
				{
					case Piece.PieceType.King:
						return (Image)m_ImageList[2];
					case Piece.PieceType.Queen:
						return (Image)m_ImageList[3];
					case Piece.PieceType.Bishop:
						return (Image)m_ImageList[4];
					case Piece.PieceType.Knight:
						return (Image)m_ImageList[5];
					case Piece.PieceType.Rook:
						return (Image)m_ImageList[6];
					case Piece.PieceType.Pawn:
						return (Image)m_ImageList[7];
					default:
						return null;
				}
			else
				switch(Piece.Type)
				{
					case Piece.PieceType.King:
						return (Image)m_ImageList[8];
					case Piece.PieceType.Queen:
						return (Image)m_ImageList[9];
					case Piece.PieceType.Bishop:
						return (Image)m_ImageList[10];
					case Piece.PieceType.Knight:
						return (Image)m_ImageList[11];
					case Piece.PieceType.Rook:
						return (Image)m_ImageList[12];
					case Piece.PieceType.Pawn:
						return (Image)m_ImageList[13];
					default:
						return null;
				}
		}
	}
}

using System;
using System.Collections.Specialized;
using System.Collections;
using System.Xml;

namespace ChessLibrary
{
	
    [Serializable]
	public class Cells
	{
		Hashtable m_Cells;

		public Cells()
		{
			m_Cells = new Hashtable();	
		}

		
		private string GetKey(Cell newcell)
		{
			return ""+ newcell.row + newcell.col;
		}

		
		private string GetKey(int row, int col)
		{
			return ""+ row + col;
		}

		
		public void Add(Cell newcell)
		{
			m_Cells.Add(GetKey(newcell), newcell);	
		}

		
		public void Remove(int row, int col)
		{
			string key=GetKey(row,col);
			if (m_Cells.ContainsKey(key)) 
				m_Cells.Remove(key);	  
		}

		
		public void Clear()
		{
			m_Cells.Clear();
		}

		
		public Cell this[int row, int col]
		{
			get
			{
				return (Cell)m_Cells[GetKey(row,col)];
			}
		}

		
		public Cell this[string strloc]
		{
			get
			{
				int col=char.Parse(strloc.Substring(0,1).ToUpper())-64; 
				int row=int.Parse(strloc.Substring(1,1));				  
				return (Cell)m_Cells[GetKey(row,col)];
			}
		}

        
        public XmlNode XmlSerialize(XmlDocument xmlDoc)
        {
            XmlElement xmlNode = xmlDoc.CreateElement("Cells");

            string xml = "";
            
            for (int row = 1; row <= 8; row++)
                for (int col = 1; col <= 8; col++)
                {
                    Cell cell = this[row, col];

                    xml += XMLHelper.XmlSerialize(typeof(Cell), cell);
                }

            xmlNode.InnerXml = xml;

            
            return xmlNode;
        }

        
        public void XmlDeserialize(XmlNode xmlCells)
        {
            
            XmlNode cellXml = xmlCells.FirstChild;

            
            for (int row = 1; row <= 8; row++)
                for (int col = 1; col <= 8; col++)
                {
                    Cell cell = (Cell)XMLHelper.XmlDeserialize(typeof(Cell), cellXml.OuterXml);
                    m_Cells[GetKey(row, col)] = cell;

                    
                    cellXml = cellXml.NextSibling;
                }
        }
	}
}

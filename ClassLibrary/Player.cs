using System;
using System.Drawing;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace ChessLibrary
{
	
    [Serializable]
	public class Player
	{
		private Type m_Type;		
		private Side m_Side;		
		private string m_Name;		
		private Image m_Image;		
		private Rules m_Rules;		
		private TimeSpan m_MaxThinkTime;		

		private TimeSpan m_TotalThinkTime;	
		private DateTime m_StartTime;		
		private int	m_TotalMovesAnalyzed;	
		private bool m_GameNearEnd;			
		public enum Type{Human, Computer};

        
        internal Player()
        {
            m_Side = PlayerSide;
            m_Type = PlayerType;
            m_MaxThinkTime = new TimeSpan(0, 0, 4);	
            m_TotalThinkTime = (DateTime.Now - DateTime.Now);	
        }

		
		public Player(Side PlayerSide, Type PlayerType)
		{
			m_Side=PlayerSide;
			m_Type=PlayerType;
			m_MaxThinkTime = new TimeSpan(0,0,4);	
			m_TotalThinkTime = (DateTime.Now - DateTime.Now);	
		}

		
		public Player(Side PlayerSide, Type PlayerType, Rules rules) : this(PlayerSide,PlayerType)
		{
			m_Rules=rules;	
		}

		public void TimeStart()
		{
			m_StartTime=DateTime.Now;
		}

		
		public void TimeEnd()
		{
			m_TotalThinkTime+= (DateTime.Now - m_StartTime);
		}

		
		public void UpdateTime()
		{
		}

		
		public bool IsComputer()
		{
			return (m_Type==Type.Computer);
		}

		public void ResetTime()
		{
			m_TotalThinkTime = (DateTime.Now - DateTime.Now);	
		}

        
        [XmlIgnore]
        internal Rules GameRules
        {
            set { m_Rules = value; }
        }

		
		public Move GetFixBestMove()
		{
			int alpha, beta;
			int depth;					
			TimeSpan ElapsedTime= new TimeSpan(1);		
			Move BestMove=null;		

			// Initialize constants
			const int MIN_SCORE= -1000000;		
			const int MAX_SCORE= 1000000;		

			ArrayList TotalMoves=m_Rules.GenerateAllLegalMoves(m_Side); 
			ArrayList PlayerCells = m_Rules.ChessBoard.GetSideCell(m_Side.type);

			alpha = MIN_SCORE;	
			beta  = MAX_SCORE;	

			depth=3;

			
			foreach (Move move in TotalMoves)
			{
				
				m_Rules.ExecuteMove(move);
				move.Score = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(m_Side).PlayerSide,depth - 1, -beta, -alpha);
				m_Rules.UndoMove(move);	

				
				
				if (move.Score > alpha)
				{
					BestMove = move;
					alpha = move.Score;
				}
			}		
			return BestMove;
		}


		
		public Move GetBestMove()
		{
			int alpha, beta;
			int depth;					
			TimeSpan ElapsedTime= new TimeSpan(1);		
			Move BestMove=null;		

			// Initialize constants
			const int MIN_SCORE= -10000000;		
			const int MAX_SCORE= 10000000;		

			ArrayList TotalMoves=m_Rules.GenerateAllLegalMoves(m_Side); 

			
			DateTime ThinkStartTime=DateTime.Now;
			int MoveCounter;
			Random RandGenerator= new Random();

			
			if (m_Rules.ChessBoard.GetSideCell(m_Side.type).Count<=5 || TotalMoves.Count <= 5 )
				m_GameNearEnd = true;

			
			Side EnemySide;

			if (m_Side.isBlack())
				EnemySide = m_Rules.ChessGame.WhitePlayer.PlayerSide;
			else
				EnemySide = m_Rules.ChessGame.BlackPlayer.PlayerSide;

			if (m_Rules.ChessBoard.GetSideCell(m_Side.Enemy()).Count<=5 || m_Rules.GenerateAllLegalMoves(EnemySide).Count <= 5 )
				m_GameNearEnd = true;

			m_TotalMovesAnalyzed=0;		

			for (depth = 1;; depth++)	
			{
				alpha = MIN_SCORE;	
				beta  = MAX_SCORE;	
				MoveCounter = 0;	

				
				foreach (Move move in TotalMoves)
				{
					MoveCounter++;

					
					m_Rules.ExecuteMove(move);
					move.Score = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(m_Side).PlayerSide,depth - 1, -beta, -alpha);
					m_TotalMovesAnalyzed++;	
					m_Rules.UndoMove(move);	

					
					if (move.Score > alpha)
					{
						BestMove = move;
						alpha = move.Score;
					}

					m_Rules.ChessGame.NotifyComputerThinking(depth, MoveCounter, TotalMoves.Count,m_TotalMovesAnalyzed, BestMove );

					
					ElapsedTime=DateTime.Now - ThinkStartTime;
					if ( ElapsedTime.Ticks > (m_MaxThinkTime.Ticks) )	
						break;							
				}

				
				ElapsedTime=DateTime.Now - ThinkStartTime;
				if ( ElapsedTime.Ticks > (m_MaxThinkTime.Ticks*0.25))	
					break;							
			}
		
			m_Rules.ChessGame.NotifyComputerThinking(depth, MoveCounter, TotalMoves.Count,m_TotalMovesAnalyzed, BestMove );
			return BestMove;
		}

		
		private int AlphaBeta(Side PlayerSide, int depth, int alpha, int beta)
		{
			int val;
			System.Windows.Forms.Application.DoEvents();

			
			int R = (depth>6 ) ? 3 : 2; 
			
			if (depth >= 2 && !m_GameNearEnd && m_Rules.ChessGame.DoNullMovePruning)	
			{
				val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide,depth  - R - 1, -beta, -beta + 1); // Try a Null Move
				if (val >= beta) 
					return beta;
			}

			
			
			bool bFoundPv = false;

			
			if (depth <= 0)
			{
				
				if (m_Rules.ChessGame.DoQuiescentSearch)
					return QuiescentSearch(PlayerSide, alpha, beta);
				else
					return m_Rules.Evaluate(PlayerSide);	
			}	
			
			ArrayList TotalMoves=m_Rules.GenerateAllLegalMoves(PlayerSide); 

			
			foreach (Move move in TotalMoves)
			{
				
				m_Rules.ExecuteMove(move);

				
				if (bFoundPv && m_Rules.ChessGame.DoPrincipleVariation) 
				{
					val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, depth - 1, -alpha - 1, -alpha);
					if ((val > alpha) && (val < beta)) 
						val=-AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide,depth - 1, -beta, -alpha); 
				} 
				else
					val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide,depth - 1, -beta, -alpha); 

				m_TotalMovesAnalyzed++;	
				m_Rules.UndoMove(move);	
			
				
				if (val >= beta)
					return beta;
				
				if (val > alpha)
				{
					alpha = val;
					bFoundPv = true;		
				}
			}
			return alpha;			
		}


		
		int QuiescentSearch(Side PlayerSide, int alpha, int beta)
		{
			int val = m_Rules.Evaluate(PlayerSide);

			if (val >= beta) 
				return beta;
			
			if (val > alpha) 
				alpha = val;

			
			ArrayList TotalMoves=m_Rules.GenerateGoodCaptureMoves(PlayerSide); 

			
			foreach (Move move in TotalMoves)
			{
				
				m_Rules.ExecuteMove(move);
				val = -QuiescentSearch(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, -beta, -alpha);
				m_Rules.UndoMove(move);	

				if (val >= beta) 
					return beta;
			
				if (val > alpha) 
					alpha = val;
			}

			return alpha;
		}


		//--------------------------------------------------
		#region Properties for the player class
        [XmlAttribute("Type=PlayerType")]
		public Type PlayerType
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
		//--------------------------------------------------
		public Side PlayerSide
		{
			get
			{
				return m_Side;
			}
			set
			{
				m_Side=value;
			}
		}
		//--------------------------------------------------
		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name=value;
			}
		}
		//--------------------------------------------------
        [XmlIgnore]
		public Image Image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image=value;
			}
		}

		
		public int TotalThinkTime
		{
			get
			{
				return m_MaxThinkTime.Seconds;	
			}
			set
			{
				m_MaxThinkTime=new TimeSpan(0,0,value);	
			}
		}

		
        [XmlIgnore]
		public TimeSpan ThinkSpanTime
		{
			get
			{
				return m_TotalThinkTime;	
			}
            set
            {
                m_TotalThinkTime = value;
            }
		}

        
        public long ThinkSpanTimeInSeconds
        {
            get
            {
                return (long)m_TotalThinkTime.TotalSeconds;	
            }
            set
            {
                m_TotalThinkTime = new TimeSpan(0,0, (int)value);
            }
        }

		
		public string ThinkTime
		{
			get
			{
				string strThinkTime;

                
                if (m_StartTime.Year == 1)
                    m_StartTime = DateTime.Now;

				TimeSpan timespan = m_TotalThinkTime+(DateTime.Now - m_StartTime);
				strThinkTime =  timespan.Hours.ToString("00")+":"+timespan.Minutes.ToString("00")+":"+timespan.Seconds.ToString("00");
				return strThinkTime;	
			}
		}
		#endregion
	}
}

using System;
using System.Runtime.InteropServices;  

namespace Chess
{
	
	public class Sounds
	{
		private string m_ParentFolder;
	
		
		private int SND_ASYNC    = 0x0001;     
		private int SND_FILENAME = 0x00020000; 
		private int SND_PURGE    = 0x0040;     

		public Sounds(string folder)
		{	
			m_ParentFolder=folder;
		}

		
		[DllImport("WinMM.dll")]
		public static extern bool  PlaySound(string fname, int Mod, int flag);

		
		public void Play(string FileName)
		{
			int SoundFlags=SND_ASYNC | SND_FILENAME;
			PlaySound(FileName, 0, SoundFlags);
		}

		
		public void StopPlay()
		{
			PlaySound(null, 0, SND_PURGE);
		}

		
		public void PlayClick()
		{
			StopPlay();
			Play(m_ParentFolder+"click.wav");
		}


		public void PlayNormalMove()
		{
			StopPlay();
			Play(m_ParentFolder+"normal_move.wav");
		}

		
		public void PlayCaptureMove()
		{
			StopPlay();
			Play(m_ParentFolder+"capture_move.wav");
		}

		
		public void PlayCheck()
		{
			StopPlay();
			Play(m_ParentFolder+"check.wav");
		}

		
		public void PlayGameOver()
		{
			StopPlay();
			Play(m_ParentFolder+"game_over.wav");
		}

	}
}

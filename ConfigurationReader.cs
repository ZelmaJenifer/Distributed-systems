namespace RabbitConfiguration
{

	using System;
	using System.IO;
	using System.Collections;


	class ConfigurationReader
	{
		private string host;
		private string server;
		private string user;
		private string password;

		/*
			This method get a line and search 
		*/
		private void SetupConfiguration(string line, int start, int length)
		{
			if(line.IndexOf("user") != -1)
			{
				this.SetUser(line.Substring(start, length));
			} 	
			else
			{
				if(line.IndexOf("password") != -1)
				{
					this.SetPassword(line.Substring(start, length));
				}
				else
				{
					if(line.IndexOf("server") != -1)
					{
						this.SetServer(line.Substring(start, length));
						
					}//end if host
					else
					{
						if(line.IndexOf("remote") != -1)
						{
							this.SetHost(line.Substring(start, length));	
						}
					}		
				}//end if passwd
						
			}//end if user

		}

		public ConfigurationReader(string file_path = "configure.txt")
		{
			StreamReader file_handler = new StreamReader(file_path);

			string line = "";

			while( line != null)
			{
				line = file_handler.ReadLine();

				if(line == null) break;

				int start 	= 	line.IndexOf(":") + 1;
				int length 	= 	line.Length - start;

				this.SetupConfiguration(line, start, length);

			}

			file_handler.Close();
		}

		public void SetServer(string server)
		{
			this.server = server;
		}

		public void SetHost(string host)
		{
			this.host = host;
		}

		public void SetUser(string user)
		{
			this.user = user;
		}

		public void SetPassword(string password)
		{
			this.password = password;
		}

		public string GetHost()
		{
			return this.host;
		}

		public string GetServer()
		{
			return this.server;
		}

		public string GetUser()
		{
			return this.user;
		}

		public string GetPassword()
		{
			return this.password;
		}

	}

}//finish namespace
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
			line (string): contains the setup to apply
			start (int): the initial position in the string to get the configuration parameter
			length (int): the length of the configuration parameter
		*/
		private void SetupConfiguration(string line, int start, int length)
		{
			//search for user
			if(line.IndexOf("user") != -1)
			{
				//sets the user name
				this.SetUser(line.Substring(start, length));
			} 	
			else 
			{
				//search for password
				if(line.IndexOf("password") != -1)
				{
					//sets the password user
					this.SetPassword(line.Substring(start, length));
				}
				else
				{
					//search for the server
					if(line.IndexOf("server") != -1)
					{
						//sets the server
						this.SetServer(line.Substring(start, length));
						
					}
					else
					{
						//search for the remote host
						if(line.IndexOf("remote") != -1)
						{
							//sets the remote host
							this.SetHost(line.Substring(start, length));	
						}//end if remote
						
					}//end if server	
					
				}//end if passwd
						
			}//end if user

		}

		/*
			This method reads the parameters file to setup the initial configuration
			file_path (string): contains the name of the configuration file, default "configure.txt"
		*/
		
		public ConfigurationReader(string file_path = "configure.txt")
		{
			//the reader to manage the configuration file
			StreamReader file_handler = new StreamReader(file_path);

			string line = "";

			//read each line
			while( line != null)
			{
				line = file_handler.ReadLine();

				if(line == null) break;

				int start 	= 	line.IndexOf(":") + 1;
				int length 	= 	line.Length - start;

				//call the setup method with the current line as a parameter
				this.SetupConfiguration(line, start, length);

			}

			file_handler.Close();
		}

		/*
			Setter for the server.
			server (string): name of the server.
		*/
		public void SetServer(string server)
		{
			this.server = server;
		}

		/*
			Setter for the host.
			host (string): address host.
		*/
		public void SetHost(string host)
		{
			this.host = host;
		}

		/*
			Setter for the user.
			user (string): name of the user.
		*/
		public void SetUser(string user)
		{
			this.user = user;
		}

		/*
			Setter for the password.
			password (string): user's password.
		*/
		public void SetPassword(string password)
		{
			this.password = password;
		}
		
		/*
			Getter for the host.
			host: returns the host.
		*/
		public string GetHost()
		{
			return this.host;
		}

		/*
			Getter for the server.
			host: returns the server.
		*/
		public string GetServer()
		{
			return this.server;
		}

		/*
			Getter for the user.
			host: returns the user.
		*/
		public string GetUser()
		{
			return this.user;
		}

		/*
			Getter for the password.
			host: returns the password.
		*/
		public string GetPassword()
		{
			return this.password;
		}

	}

}//finish namespace
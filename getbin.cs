

using System;
using System.IO;
using System.Net;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using NDesk.Options;

public class Driver {

	public static int Main (string [] args)
	{
		Driver driver = new Driver ();
		bool help = false;

		var p = new OptionSet () {
				{ "h|?|help", v => help = v != null },
				{ "pb|pastbin-url", v => driver.PasteBinUrl = v },
				{ "r|redirect-stdout", v => driver.RedirectToStdout = v != null },
				{ "o|overwrite-existing", v => driver.OverwriteExisting = v != null },
				{ "v|verbose", v => driver.Verbose =  v != null },
		};

		List<string> extra = null;
		try {
			extra = p.Parse (args);
		} catch (OptionException){
			Console.WriteLine ("Try `getbin --help' for more information.");
			return 1;
		}

		if (help){
			ShowHelp (p);
			return 0;
		}

		if (extra.Count < 1) {
			ShowHelp (p);
			return -1;
		}

		if (extra.Count > 1)
			driver.FileName = extra [1];
		driver.Url = extra [0];

		driver.GetBin ();
		return 0;
	}

	public static void ShowHelp (OptionSet os)
	{
		Console.WriteLine ("getbin usage is: getbin [options] [directory]");
		Console.WriteLine ();
		os.WriteOptionDescriptions (Console.Out);
	}

	private string file_name;
	private string url;

	private string pastebin_url;
	private bool verbose;
	private bool overwrite_existing;
	private bool redirect;

	public string FileName {
		get { return file_name; }
		set { file_name = value; }
	}

	public string Url {
		get { return url; }
		set { url = value; }
	}
	
	public string PasteBinUrl {
		get { return pastebin_url; }
		set { pastebin_url = value; }
	}

	public bool Verbose {
		get { return verbose; }
		set { verbose = value; }
	}

	public bool OverwriteExisting {
		get { return overwrite_existing; }
		set { overwrite_existing = value; }
	}

	public bool RedirectToStdout {
		get { return redirect; }
		set { redirect = value; }
	}

	public Driver ()
	{
		PasteBinUrl = ConfigurationManager.AppSettings ["PasteBinUrl"];

		bool b = false;
		if (Boolean.TryParse (ConfigurationManager.AppSettings ["RedirectToStdout"], out b))
			RedirectToStdout = b;

		if (Boolean.TryParse (ConfigurationManager.AppSettings ["OverwriteExisting"], out b))
			OverwriteExisting = b;

		if (Boolean.TryParse (ConfigurationManager.AppSettings ["Verbose"], out b))
			Verbose = b;
	}

	public void GetBin ()
	{
		Uri uri = BuildUri (verbose);
		string page = FetchPage (uri);
		string pastecontents = GetPasteContents (page);

		if (redirect) {
			Console.WriteLine (pastecontents);
			return;
		}

		SaveContents (pastecontents);
	}

	public Uri BuildUri (bool verbose)
	{
		UriBuilder builder;

		if (Char.IsDigit (url [0])) {
			builder = new UriBuilder ();
			builder.Scheme = "http";
			builder.Host = PasteBinUrl;
			builder.Path = String.Concat ("/raw/", url);
		} else {
			builder = new UriBuilder (url);

			if (!builder.Path.StartsWith ("/raw/"))
				builder.Path = String.Concat ("/raw/", builder.Path);
		}

		if (verbose)
			Console.WriteLine ("getbin: built uri:  {0}", builder.Uri);

		return builder.Uri;
	}

	public string FetchPage (Uri uri)
	{
		WebClient client = new WebClient ();
		string res;

		client.Headers.Add ("user-agent", "getbin");

		if (verbose)
			Console.WriteLine ("getbin: about to fetch page");

		using (Stream data = client.OpenRead (uri)) {
			using (StreamReader reader = new StreamReader (data)) {
				res = reader.ReadToEnd ();
				data.Close ();
				reader.Close ();
			}
		}

		if (verbose)
			Console.WriteLine ("getbin: fetched page");

		return res;
	}

	public string GetPasteContents (string str)
	{
		return str;
	}

	public void SaveContents (string contents)
	{
		EnsureFileName ();

		if (verbose)
			Console.WriteLine ("getbin: saving to file: '{0}'", file_name);
		File.WriteAllText (file_name, contents);
	}

	private void EnsureFileName ()
	{
		if (file_name == null) {
			file_name = Path.GetFileName (BuildUri (false).AbsolutePath);

			if (!overwrite_existing) {
				int i = 1;
				string orig = file_name;
				while (File.Exists (file_name)) {
					file_name = String.Concat (orig, ".", i);
					i++;
				}
			}
		}

	}
}



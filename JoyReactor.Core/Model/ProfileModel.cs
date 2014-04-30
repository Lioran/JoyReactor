﻿using System;
using System.Threading.Tasks;
using System.Threading;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.Model.Inject;
using System.Linq;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace JoyReactor.Core.Model
{
	public class ProfileModel : IProfileModel
	{
		private ISiteParser[] parsers = InjectService.Instance.Get<ISiteParser[]>();

		#region IProfileModel implementation

		public Task LoginAsync (string username, string password)
		{
			return Task.Run(() => {
				var p = parsers.First(s => s.ParserId == ID.SiteParser.JoyReactor);
				var c = p.Login(username, password);

				if (c == null || c.Count < 1) throw new Exception();

				var pf = new Profile { Cookie = SerializeObject(c), Site = "" + ID.SiteParser.JoyReactor, Username = username };
				MainDb.Instance.InsertOrReplace(pf);
			});
		}

		public Task LogoutAsync ()
		{
			return Task.Run (() => {
				MainDb.Instance.Execute("DELETE FROM profiles");
			});
		}

		public Task<ProfileInformation> GetCurrentProfileAsync ()
		{
			return Task.Run (() => {
//				ThreadHelper.Sleep(1000);
//				return new ProfileInformation();
//				return (ProfileInformation)null;

				var un = MainDb.Instance.ExecuteScalar<string>("SELECT Username From profiles WHERE Site = ?", "" + ID.SiteParser.JoyReactor);
				if (un == null) return null;

				var p = parsers.First(s=>s.ParserId==ID.SiteParser.JoyReactor);
				var pf = p.Profile(un);

				return new ProfileInformation {
					Username = pf.Username,
					Rating = pf.Rating,
				};
			});
		}

		#endregion

		#region Private methods

		private static string SerializeObject(IDictionary<string, string> o) 
		{
			return o.Aggregate ("", (a, s) => a + (a.Length > 0 ? ";" : "") + s.Key + "=" + s.Value);
		}

		private static IDictionary<string, string> DeserializeObject<T>(string o) 
		{
			return o.Split (';').Select (s => s.Split ('=')).ToDictionary (s => s [0], s => s [1]);
		}

		#endregion
	}
}
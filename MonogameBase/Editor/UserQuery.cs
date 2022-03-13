using System.Linq;

namespace MonogameBase
{
    using System.IO;

    public class UserQuery
    {
        private readonly Svara.Query svar;

        public UserQuery()
        {
            svar = new Svara.Query("tt.txt",true);
        }

        public string GetUserInput(string msg)
        {
            return svar.GetUserInput(msg).answer;
        }

        public string GetLevelName()
        {
            var levels = Directory.GetFiles("./", "*.level").Select(x => $"{x}");

            return svar.GetUserInput(levels).answers.First();
        }

        public (string level,string name, string spawn) GetGateData()
        {
            var res = svar.GetUserInput("set gate data: ToLevel,Name,SpawnPoint");
            var ans = res.answer.Split(",").Select(c => c.Trim()).ToList();
            return (ans[0], ans[1], ans[2]); 
        }


    }
}
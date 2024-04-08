using BasketBetWebAPI.Exceptions;
using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Globalization;

namespace BasketBetWebAPI.Services
{
    public class Scrapper
    {
        private const string _standingsUrl = "https://www.espn.com/nba/standings";
        private const string _scheduleUrl = "https://www.espn.co.uk/nba/fixtures";

        private readonly ITeamsRepository _teamsRepository;
        private readonly IGamesRepository _gamesRepository;

        private List<TeamDto> teamDtos { get; set; }
        private List<GameDto> gameDtos { get; set; }

        public Scrapper(ITeamsRepository teamsRepository, IGamesRepository gamesRepository)
        {
            _teamsRepository = teamsRepository;
            _gamesRepository = gamesRepository;
            teamDtos = new List<TeamDto>();
            gameDtos = new List<GameDto>();
        }
        
        public async Task UpdateTable()
        {
            teamDtos.Clear();
            var web = new HtmlWeb();
            var document = web.Load(_standingsUrl);

            var easternTableNames = document.QuerySelector("table tbody");
            var easterntableparent = easternTableNames?.ParentNode.ParentNode;
            var easternTableStats = easterntableparent?.LastChild.QuerySelector("table tbody");

            for (int i = 0; i < easternTableNames.ChildNodes.Count; i++)
            {
                string wPercantage = $"0{easternTableStats.ChildNodes[i].ChildNodes[2].InnerText}";
                Double.TryParse(wPercantage, NumberStyles.Float, CultureInfo.InvariantCulture, out double wPercantageDouble);
                string ppg = $"0{easternTableStats.ChildNodes[i].ChildNodes[8].InnerText}";
                Double.TryParse(ppg, NumberStyles.Float, CultureInfo.InvariantCulture, out double ppgDouble);
                string oppg = $"0{easternTableStats.ChildNodes[i].ChildNodes[9].InnerText}";
                Double.TryParse(oppg, NumberStyles.Float, CultureInfo.InvariantCulture, out double oppgDouble);
                teamDtos.Add(new TeamDto
                {
                    Name = easternTableNames.ChildNodes[i].FirstChild.FirstChild.LastChild.FirstChild.InnerText,
                    Wins = int.Parse(easternTableStats.ChildNodes[i].ChildNodes[0].InnerText),
                    Looses = int.Parse(easternTableStats.ChildNodes[i].ChildNodes[1].InnerText),
                    WinningPercentage = wPercantageDouble,
                    HomeRecord = easternTableStats.ChildNodes[i].ChildNodes[4].InnerText,
                    AwayRecord = easternTableStats.ChildNodes[i].ChildNodes[5].InnerText,
                    PointsPerGame = ppgDouble,
                    OpponentPointsPerGame = oppgDouble,
                    CurrentStreak = easternTableStats.ChildNodes[i].ChildNodes[11].InnerText,
                    Last10Record = easternTableStats.ChildNodes[i].ChildNodes[12].InnerText,
                    Conference = "Eastern"
                });
            }


            var westernTableNames = document.QuerySelectorAll("table tbody")[2];

            var westerntableparent = westernTableNames?.ParentNode.ParentNode;
            var westernTableStats = westerntableparent?.LastChild.QuerySelector("table tbody");

            for (int i = 0; i < westernTableNames.ChildNodes.Count; i++)
            {
                string wPercantage = $"0{westernTableStats.ChildNodes[i].ChildNodes[2].InnerText}";
                Double.TryParse(wPercantage, NumberStyles.Float, CultureInfo.InvariantCulture, out double wPercantageDouble);
                string ppg = $"0{westernTableStats.ChildNodes[i].ChildNodes[8].InnerText}";
                Double.TryParse(ppg, NumberStyles.Float, CultureInfo.InvariantCulture, out double ppgDouble);
                string oppg = $"0{westernTableStats.ChildNodes[i].ChildNodes[9].InnerText}";
                Double.TryParse(oppg, NumberStyles.Float, CultureInfo.InvariantCulture, out double oppgDouble);
                teamDtos.Add(new TeamDto
                {
                    Name = westernTableNames.ChildNodes[i].FirstChild.FirstChild.LastChild.FirstChild.InnerText,
                    Wins = int.Parse(westernTableStats.ChildNodes[i].ChildNodes[0].InnerText),
                    Looses = int.Parse(westernTableStats.ChildNodes[i].ChildNodes[1].InnerText),
                    WinningPercentage = wPercantageDouble,
                    HomeRecord = westernTableStats.ChildNodes[i].ChildNodes[4].InnerText,
                    AwayRecord = westernTableStats.ChildNodes[i].ChildNodes[5].InnerText,
                    PointsPerGame = ppgDouble,
                    OpponentPointsPerGame = oppgDouble,
                    CurrentStreak = westernTableStats.ChildNodes[i].ChildNodes[11].InnerText,
                    Last10Record = westernTableStats.ChildNodes[i].ChildNodes[12].InnerText,
                    Conference = "Western"
                });
            }
            await _teamsRepository.Update(teamDtos);

        }
        public async Task UpdateGames()
        {
            gameDtos.Clear();
            var web = new HtmlWeb();
            var document = web.Load(_scheduleUrl);

            var tables = document.QuerySelectorAll("table.schedule");
            List<string> captions = new();


            var parent = tables[0].ParentNode.ParentNode;

            var children = parent.ChildNodes;

            for (int j = 1; j < children.Count; j++)
            {
                var currentElement = children[j];

                if (currentElement.Name == "div")
                {
                    var spanIndex = j - 1;
                    if (spanIndex >= 0 && ( children[spanIndex].Name == "span" || children[spanIndex].Name == "h2"))
                    {
                        var spanText = children[spanIndex].InnerText;
                        captions.Add(spanText);

                        var nextElementIndex = j + 1;
                        if (nextElementIndex < children.Count && children[nextElementIndex].Name == "div")
                        {
                            captions.Add(spanText);
                        }
                    }
                }
            }


            Thread.Sleep(10);
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].InnerText == "No games scheduled") continue;

                string dateFormatSingle = "dddd, d MMMM";
                string dateFormatDouble = "dddd, dd MMMM";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                DateTime parsedDate;
                DateOnly dateOnly;
                if (DateTime.TryParseExact(captions[i], dateFormatSingle, culture, DateTimeStyles.None, out parsedDate))
                {
                    dateOnly = new DateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);
                }
                else if (DateTime.TryParseExact(captions[i], dateFormatDouble, culture, DateTimeStyles.None, out parsedDate))
                {
                    dateOnly = new DateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);
                }
                else
                {
                    continue;
                }
                var trs = tables[i].SelectNodes(".//tr");


                for (int j = 1; j < trs.Count; j++)
                {
                    var tr = trs[j];
                    var awayTeamDto = await _teamsRepository.GetByName(tr.FirstChild.LastChild.FirstChild.InnerText);
                    var homeTeamDto = await _teamsRepository.GetByName(tr.ChildNodes[1].FirstChild.LastChild.FirstChild.InnerText);
                    var Odds = CreateOdds(awayTeamDto.WinningPercentage, homeTeamDto.WinningPercentage);

                    gameDtos.Add(new GameDto
                    {
                        Date = dateOnly,
                        AwayTeamDto = awayTeamDto,
                        AwayTeamDtoId = awayTeamDto.Id,
                        OddsAwayTeam = Odds[0],
                        HomeTeamDto = homeTeamDto,
                        HomeTeamDtoId = homeTeamDto.Id,
                        OddsHomeTeam = Odds[1],
                    });
                }


            }
           
            await _gamesRepository.UpdateGames(gameDtos);
        }

        public async Task UpdateGamesFromDate(DateOnly date)
        {
            gameDtos.Clear();

            string formattedDate = date.ToString("yyyyMMdd");
            string Url = $"https://www.espn.co.uk/nba/fixtures/_/date/{formattedDate}";
            var web = new HtmlWeb();
            var document = web.Load(Url);

            var tables = document.QuerySelectorAll("table.schedule");

            List<string> captions = new ();

            
            var parent = tables[0].ParentNode.ParentNode;

            var children = parent.ChildNodes;

            for (int j = 1; j < children.Count; j++)
            {
                var currentElement = children[j];

                if (currentElement.Name == "div")
                {
                    var spanIndex = j - 1;
                    if (spanIndex >= 0 && children[spanIndex].Name == "span")
                    {
                        var spanText = children[spanIndex].InnerText;
                        captions.Add(spanText);

                        var nextElementIndex = j + 1;
                        if (nextElementIndex < children.Count && children[nextElementIndex].Name == "div")
                        {
                            captions.Add(spanText);
                        }
                    }
                }
            }

            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].InnerText == "No games scheduled") continue;

                string dateFormatSingle = "dddd, d MMMM";
                string dateFormatDouble = "dddd, dd MMMM";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                DateTime parsedDate;
                DateOnly dateOnly;
                if (DateTime.TryParseExact(captions[i], dateFormatSingle, culture, DateTimeStyles.None, out parsedDate))
                {
                    dateOnly = new DateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);                    
                }
                else if (DateTime.TryParseExact(captions[i], dateFormatDouble, culture, DateTimeStyles.None, out parsedDate))
                {
                    dateOnly = new DateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);                    
                }
                else
                {
                    continue;
                }
                var trs = tables[i].SelectNodes(".//tr");

                
                for (int j = 1; j < trs.Count; j++)
                {
                    var tr = trs[j];
                    var awayTeamDto = await _teamsRepository.GetByName(tr.FirstChild.LastChild.FirstChild.InnerText);
                    var homeTeamDto = await _teamsRepository.GetByName(tr.ChildNodes[1].FirstChild.LastChild.FirstChild.InnerText);
                    var Odds = CreateOdds(awayTeamDto.WinningPercentage, homeTeamDto.WinningPercentage);

                    gameDtos.Add(new GameDto
                    {
                        Date = dateOnly,
                        AwayTeamDto = awayTeamDto,
                        AwayTeamDtoId = awayTeamDto.Id,
                        OddsAwayTeam = Odds[0],
                        HomeTeamDto = homeTeamDto,
                        HomeTeamDtoId = homeTeamDto.Id,
                        OddsHomeTeam = Odds[1],
                    });
                }
                

            }

            await _gamesRepository.UpdateGames(gameDtos);
        }

        public async Task<bool> UpdateGamesResults(DateOnly date)
        {
            int result;
            gameDtos.Clear();
            string formattedDate = date.ToString("yyyyMMdd");
            string Url = $"https://www.espn.co.uk/nba/scoreboard/_/date/{formattedDate}";

            var web = new HtmlWeb();
            var document = web.Load(Url);

            var module = document.QuerySelector(".gameModules");

            if (module.LastChild.FirstChild.InnerText != "No games on this date.")
            {
                var lis = module.QuerySelectorAll("li");
                for (int i = 0; i < lis.Count; i += 2)
                {
                    int? awayTeamScore;
                    int? homeTeamScore;

                    var awayTeamName = lis[i].ChildNodes[1].FirstChild.InnerText;
                    var awayScoreString = lis[i].QuerySelector(".ScoreCell__Score") ?? null;
                    if (awayScoreString != null)
                    {
                        awayTeamScore = int.Parse(awayScoreString.InnerText);
                    }
                    else awayTeamScore = null;

                    var homeTeamName = lis[i + 1].ChildNodes[1].FirstChild.InnerText;
                    var homeScoreString = lis[i+1].QuerySelector(".ScoreCell__Score") ?? null;
                    if (homeScoreString != null)
                    {
                        homeTeamScore = int.Parse(homeScoreString.InnerText);
                    }
                    else homeTeamScore = null;

                    gameDtos.Add(
                        new GameDto
                        {
                            AwayTeamDto = new TeamDto { Name = awayTeamName },
                            AwayTeamScore = awayTeamScore,
                            HomeTeamDto = new TeamDto { Name = homeTeamName },
                            HomeTeamScore = homeTeamScore,
                            Date = date
                        }
                        );

                }
                
                
            }
            result = await _gamesRepository.UpdateGamesScores(gameDtos);
            return result> 0;


        }
        private double[] CreateOdds(double winPercentageTeam1, double winPercentageTeam2)
        {
            double sum = winPercentageTeam1 + winPercentageTeam2;

            double oddsTeam1 = 0.95 * (sum / winPercentageTeam1);
            double oddsTeam2 = 0.95 * (sum / winPercentageTeam2);

            return [oddsTeam1, oddsTeam2];
        }

    }
}

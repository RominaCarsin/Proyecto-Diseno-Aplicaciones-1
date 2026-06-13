Clase: Confederation

- AddTeam(Team team)
- RemoveTeam(Team team)

Clase: Group

- AddTeam(Team? team)
- FinishMatch(Match match)
- AddMatch(Match? match)
- SetMatchResult(Match match, int homeGoals, int awayGoals)
- GetTeamByPosition(int position)
- GetOrderedTeamsByPosition()
- FinishPlayedMatches()

Clase: GroupPhase

- Generate()

Clase: GroupStageSimulator

- Simulate(IEnumerable<Group> groups)

Clase: Knockout

- UpdateSimulationSeed(int newSeed)
- SetQualifiedTeams(IEnumerable<Team> qualifiedTeams)
- InitializeRoundOf32()
- AddMatch(Match match)
- InitializeRoundOf16()
- InitializeQuarterFinals()
- InitializeSemiFinals()
- InitializeThirdPlace()
- InitializeFinal()
- LoadExistingMatches(IEnumerable<Match> matches)
- SetMatchResult(string matchCode, int homeGoals, int awayGoals)

Clase: KnockoutSimulator

- Simulate(IEnumerable<Match> matches)

Clase: Match

- SetResult(int homeGoals, int awayGoals)
- Finish()
- UpdateDate(DateTime newDate)
- UpdateStadium(Stadium? newStadium)
- RegisterIncidence(MatchIncidence incidence)
- YellowCardsOf(Team team)
- RedCardsOf(Team team)

Clase: Pot

- IsFull()

Clase: RankingCalculator

- CalculateNewRanking(int teamRanking, int opponentRanking, double result, TournamentPhase phase)
- CalculateDelta(int teamRanking, int opponentRanking, double result, TournamentPhase phase)

Clase: Stadium

- UpdateName(string name)
- UpdateCity(string city)
- UpdateDescription(string? description)
- UpdateCapacity(int capacity)

Clase: Team

- UpdateName(string name)
- UpdateConfederationName(string confederationName)
- UpdateRankingFifa(int rankingFifa)
- UpdateGroupLabel(string groupLabel)
- RecordWin(int goalsScored, int goalsConceded)
- RecordDraw(int goalsScored)
- RecordLoss(int goalsScored, int goalsConceded)
- RevertWin(int goalsScored, int goalsConceded)
- RevertDraw(int goalsScored)
- RevertLoss(int goalsScored, int goalsConceded)
- UpdateFlag(string flagBase64)

Clase: TextNormalizer

- Normalize(string value)

Clase: User

- UpdatePersonalInfo(string name, string surname, string email, DateTime dateOfBirth)
- SetPassword(string plainPassword)
- VerifyPassword(string plainPassword)
- AddRole(Role role)
- RemoveRole(Role role)
- HasRole(Role role)

Clase: AuditService

- Log(string actorEmail, string action, string details)
- GetAll()

Clase: AuthStateService

- NotifyStateChanged()

Clase: SessionService

- Login(string email, string password)
- Logout()
- IsLoggedIn()
- GetCurrentUserEmail()
- GetCurrentUser()
- CurrentUserHasRole(RoleDto role)

Clase: ShuffleService

- Shuffle<T>(IList<T> list)
- ShuffleByRanking(IList<Team> teams)

Clase: StadiumService

- AddStadium(StadiumRequest stadiumRequest)
- DeleteStadium(string stadiumName)
- UpdateStadium(string originalStadiumName, StadiumRequest updatedStadiumData)
- GetAllStadiums()

Clase: TeamService

- AddTeam(TeamRequest teamRequest)
- GetTeams()
- GetTeamsOrdered(int seed)
- UpdateTeam(string currentName, TeamRequest updatedTeamData)
- DeleteTeam(string name)
- AutoCompleteTeams(int seed)
- GetConfederations()
- PreviewImport(string csvContent)
- Import(string csvContent)
- GetTeamsByRanking()

Clase: TournamentService

- GenerateFixture(int seed, DateTime startDate)
- GenerateFixture(int seed)
- CanGenerateFixture()
- GetStadiumCount()
- GetGroups()
- GetGroupByLabel(char label)
- GetGroupStandings(char groupLabel)
- GetQualifiedTeams()
- GetMatches()
- GetMatchById(int matchId)
- SetGroupMatchResult(char groupLabel, int matchId, int homeGoals, int awayGoals)
- FinishGroupMatch(char groupLabel, int matchId)
- SimulateGroupStage(int seed)
- InitializeKnockoutStage(int simulationSeed, int semillaCrucesFase = 0)
- SetKnockoutMatchResult(string matchCode, int homeGoals, int awayGoals)
- GetKnockoutMatches()
- InitializeRoundOf16()
- InitializeQuarterFinals()
- InitializeSemiFinals()
- InitializeThirdPlace()
- InitializeFinal()
- SimulateKnockoutStage(int seed)
- FinishAllMatches()
- AddMatch(MatchRequest matchRequest)
- SetMatchResult(int matchId, int homeGoals, int awayGoals)
- UpdateMatchDate(int matchId, DateTime newDate)
- UpdateMatchStadium(int matchId, StadiumRequest newStadiumRequest)
- FinishMatch(int matchId)
- GetAvailableStadiumsForMatch(int matchId, IEnumerable<StadiumRequest> candidateStadiums)
- RegisterMatchIncidence(int matchId, string incidenceType, bool isHomeTeam, int minute)

Clase: UpdateRankingService

- UpdateAfterMatch(Match match)

Clase: UserService

- AddUser(UserRequest userRequest)
- UpdateUser(string email, UserRequest userRequest)
- DeleteUser(string email)
- GetUserByEmail(string email)
- GetUsers()
- Login(string email, string password)
- UserHasRole(string email, RoleDto role)
- AddRoleToUser(string adminEmail, string targetEmail, RoleDto role)
- RemoveRoleFromUser(string adminEmail, string targetEmail, RoleDto role)
- ChangePassword(string email, string currentPassword, string newPassword)
- ResetPassword(string adminEmail, string targetEmail, string newPassword)

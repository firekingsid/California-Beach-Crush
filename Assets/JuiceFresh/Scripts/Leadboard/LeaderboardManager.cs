using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LeaderboardManager : MonoBehaviour
{
	public static LeaderboardManager Instance;

	private static bool isAuthenticated;
	public static bool IsAuthenticated
	{
		get { return isAuthenticated; }
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}

	// Use this for initialization
	void Start()
	{
		Social.localUser.Authenticate(ProcessAuthentication);
	}

	void ProcessAuthentication(bool success)
	{
		isAuthenticated = success;
		if (success)
		{
			print("Authenticated Successfully.");
			print("UserName : " + Social.localUser.userName + " , ID : " + Social.localUser.id);
			Social.localUser.LoadFriends((bool isSuccess) =>
			{
				if (isSuccess)
				{
					print("Friends Count : " + Social.localUser.friends.Length);
				}
			});
		}
		else
		{
			print("Authenticated Failed.");
		}
	}

	public void PostScore(int level, int score, Transform container = null)
	{
		string LeaderboardID = "Level_" + level;
		Social.ReportScore(score, LeaderboardID, (result) =>
		{
			if (result)
			{
				print("Score Posted Successfully");
				if (container != null)
				{
					StartCoroutine(UpdateLeaderboardPanel(container, level));
				}
			}
			else
			{
				print("Score Posted Failed");
			}
		});
	}

	IEnumerator UpdateLeaderboardPanel(Transform container, int level)
	{
		yield return new WaitForSeconds(1.5f);
		InitLeaderboardPanel(container, level);
	}

	public void InitLeaderboardPanel(Transform container, int level)
	{
		ILeaderboard leaderboard = Social.CreateLeaderboard();
		leaderboard.id = "Level_" + level;
		leaderboard.timeScope = TimeScope.AllTime;
		leaderboard.userScope = UserScope.Global;
		leaderboard.LoadScores((success) =>
		{
			if (success)
			{
				var scores = leaderboard.scores;
				print("Load Leaderbaord Success : LV-" + level);
				print("Score Length : " + scores.Length);
				if (scores.Length > 0)
				{
					GameObject LBItem = Resources.Load<GameObject>("Prefabs/LBUserItem");
					List<string> userIDs = new List<string>();
					foreach (var score in scores)
					{
						userIDs.Add(score.userID);
					}
					Social.LoadUsers(userIDs.ToArray(), (userList) =>
					{
						if (level == LevelManager.Instance.currentLevel)
						{
							for (int i = 0; i < userList.Length; i++)
							{
								var score = scores[i];
								GameObject item = Instantiate(LBItem);
								item.transform.SetParent(container);
								item.transform.localScale = Vector3.one;
								item.transform.localPosition = Vector3.zero;

								item.GetComponent<LBItem>().Init(userList[i].image, userList[i].userName, score.value.ToString(), score.rank);
							}
						}
					});
				}
			}
			else
			{
				print("Error Loading Leaderboard");
			}
		});
	}
}

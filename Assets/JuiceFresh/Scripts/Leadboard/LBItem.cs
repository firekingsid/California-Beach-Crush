using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class LBItem : MonoBehaviour
{

	public Image profileImage;
	public Text score;
	public Text rank;
	public Text userID;

	string photoURL;

	public void Init(Texture2D userImage, string ID, string SC, int RK, string FBID = null)
	{
		userID.text = ID;
		score.text = SC;
		rank.text = RK.ToString();
		if (FBID != null)
		{
			photoURL = FBID;
			print("PhotoURL:" + photoURL);
			FB.API("/" + photoURL + "/picture?g&width=128&height=128&redirect=false", HttpMethod.GET, this.ProfilePhotoCallback);
		}
		//else if (userImage != null)
		//{
		//	Sprite sprite = Sprite.Create(userImage, new Rect(0, 0, userImage.width, userImage.height), new Vector2(0, 0));
		//	profileImage.sprite = sprite;
		//}
	}

	private void ProfilePhotoCallback(IGraphResult result)
	{
		if (string.IsNullOrEmpty(result.Error))//1.4.6
		{
			if (result.Texture != null)
			{
				Sprite sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0, 0));
				profileImage.sprite = sprite;
			}
		}
		else
		{
			print("Photo Callback NULL");
		}
	}
}

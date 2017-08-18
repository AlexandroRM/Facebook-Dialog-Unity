using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Facebook.Unity;
using System;

public class ShareImageFacebook : MonoBehaviour 
{
	byte[] screenshot;
	string text;
	FacebookDelegate<IGraphResult> shareResponseCallback;

	void Awake ()
	{
		if (!FB.IsInitialized) 
		{
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} 
		else 
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized)
		{
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} 
		else 
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} 
		else 
		{
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	#region toShareWithFacebook

	public void shareImageAndText(byte[] screenshot, string text, FacebookDelegate<IGraphResult> shareResponseCallback)
	{
		this.screenshot = screenshot;
		this.text = text;
		this.shareResponseCallback = shareResponseCallback;
		LoginWithPublishPermission();
	}

	void LoginWithPublishPermission()
	{
		var perms = new List<string>() { "publish_actions" };
		FB.LogInWithPublishPermissions(perms, AuthWithPublishPermission);
	}

	private void AuthWithPublishPermission(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log("permission "+perm);
			}
			sendDataToFacebook ();
		}
		else
		{
			Debug.Log("User cancelled login");
		}
	}

	void sendDataToFacebook()
	{
		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "Screenshot.png");
		wwwForm.AddField("message",text); 

		FB.API("me/photos", HttpMethod.POST, shareResponseCallback, wwwForm);
	}

	void shareResponse(IGraphResult result)
	{
		if (result.Error != null) 
		{
			Debug.LogError ("errors " + result.Error + " " + result.RawResult);
			return;
		}

		Debug.LogError("image was shared");
	}
	#endregion

	//those methods are not used by Controller
	#region 

	void ShareCallback (IShareResult result)
	{
		if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
		{
			Debug.Log("ShareLink Error: "+result.Error);
		} 
		else if (!String.IsNullOrEmpty(result.PostId)) 
		{
			// Print post identifier of the shared content
			Debug.Log(result.PostId);
		} 
		else 
		{
			// Share succeeded without postID
			Debug.Log("ShareLink success!");
		}
	}

	void LoginWithReadPermission()
	{
		var perms = new List<string>() { "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log("alex permission "+perm);
			}
		}
		else
		{
			Debug.Log("User cancelled login");
		}
	}
	#endregion
}
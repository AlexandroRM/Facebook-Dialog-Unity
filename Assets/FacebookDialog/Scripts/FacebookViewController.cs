using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class FacebookViewController : MonoBehaviour 
{
	public UnityEngine.UI.Image image;
	public UnityEngine.UI.InputField input;
	public GameObject dialog;
	public Button btnFacebook;
	public Button btnBack;
	private byte[] screenshot;

	// Use this for initialization
	void Start () 
	{
	}

	void buttonsActive(bool state)
	{
		btnBack.gameObject.SetActive(state);
		btnFacebook.gameObject.SetActive(state);
	}

	public void showDialog()
	{
		dialog.SetActive (true);
		dialog.GetComponent<Animator> ().Play ("ShowDialog", -1, 0f);
		//prepareForScreenshot();
	}

	public void hideDialog()
	{
		dialog.SetActive (false);
		dialog.GetComponent<Animator> ().Play ("HideDialog", -1, 0f);
	}

	public void cancel()
	{
		input.text = null;
		screenshot = null;
		image.material.mainTexture = null;
		hideDialog();
		buttonsActive(true);
	}

	public void share()
	{
		string text = input.text == null ? "" : input.text;
		this.GetComponent<ShareImageFacebook> ().shareImageAndText(screenshot, text, shareResponse);
	}

	public void prepareForScreenshot()
	{
		buttonsActive(false);
		GetComponent<AudioSource> ().Play ();
		StartCoroutine (TakeScreenshot ());
	}

	IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();

		var width = Screen.width;
		var height = Screen.height;
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

		// Read screen contents into the texture
		texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		texture.Apply();
		screenshot = texture.EncodeToPNG();
		image.material.mainTexture = texture;
		showDialog ();
	}

	void shareResponse(IGraphResult result)
	{
		if (result.Error != null) 
		{
			Debug.LogError ("errors " + result.Error + " " + result.RawResult);
			return;
		}

		Debug.LogError("image was shared");
		hideDialog ();
		buttonsActive(true);
	}
}

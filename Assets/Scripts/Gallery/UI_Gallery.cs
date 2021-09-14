using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

/* This class contains the logic of the scene "Gallery"
 * Methods:
 *		- show and delete picture
 *		- get next or previous picture
 *		- switch to the mainmenu
 */
public class UI_Gallery : MonoBehaviour
{
	// variables
	[SerializeField]
	GameObject canvas;
	[SerializeField]
	Sprite defaultImage;
	string[] files = null;
	int whichScreenShotIsShown = 0;


	void Start()
	{
		files = Directory.GetFiles(Application.persistentDataPath + "/", "*.png");

		if (files.Length > 0)
		{
			GetPictureAndShowIt();
		}
	}

	// get the path to the picture and show it
	private void GetPictureAndShowIt()
	{
		string pathToFile = files[whichScreenShotIsShown];
		Texture2D texture = GetScreenshotImage(pathToFile);
		Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
			new Vector2(0.5f, 0.5f));
		canvas.GetComponent<Image>().sprite = sp;
	}

	// return the picture with the path
	private Texture2D GetScreenshotImage(string filePath)
	{
		Texture2D texture = null;
		byte[] fileBytes;
		if (File.Exists(filePath))
		{
			fileBytes = File.ReadAllBytes(filePath);
			texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
			texture.LoadImage(fileBytes);
		}
		return texture;
	}


	/*
	 * These are all methods for the buttons in this scene
	 */

	// Delete a Screenshot
	public void DeleteImage()
	{
		if (files.Length > 0)
		{
			string pathToFile = files[whichScreenShotIsShown];
			if (File.Exists(pathToFile))
			{
				File.Delete(pathToFile);
			}

			files = Directory.GetFiles(Application.persistentDataPath + "/", "*.png");
			if (files.Length > 0)
			{
				// show the next picture
				NextPicture();
			}
			else
			{
				// show the default image
				canvas.GetComponent<Image>().sprite = defaultImage;
			}
		}
	}

	// get the next picture and show it
	public void NextPicture()
	{
		if (files.Length > 0)
		{
			whichScreenShotIsShown += 1;
			if (whichScreenShotIsShown > files.Length - 1)
				whichScreenShotIsShown = 0;
			GetPictureAndShowIt();
		}
	}

	// get the previous picture and show it
	public void PreviousPicture()
	{
		if (files.Length > 0)
		{
			whichScreenShotIsShown -= 1;
			if (whichScreenShotIsShown < 0)
				whichScreenShotIsShown = files.Length - 1;
			GetPictureAndShowIt();
		}
	}

	// change the scene
	public void ButtonMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}

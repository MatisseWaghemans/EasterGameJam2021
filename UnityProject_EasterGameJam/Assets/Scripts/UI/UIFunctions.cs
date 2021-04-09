using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIFunctions : MonoBehaviour
{

	public void OnClickPlay(string sceneName)
	{
		StartCoroutine(LoadScene(sceneName));
	}

	public void OnClickExit(float delayTime)
	{
		StartCoroutine(ExitGame(delayTime));
	}

	public IEnumerator ExitGame(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		Application.Quit();
	}

	private IEnumerator LoadScene(string sceneName)
	{

		yield return new WaitForSeconds(5f);
		Debug.Log("scene loaded : " + sceneName);

	}
}


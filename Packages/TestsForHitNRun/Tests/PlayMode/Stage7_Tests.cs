using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Description("Take the point! Affirmative."), Category("7")]
public class Stage7_Tests
{
    public GameObject score;
    public GameObject canvas;

    [UnityTest, Order(0)]
    public IEnumerator Check()
    {
        Time.timeScale = 15;
        if (!Application.CanStreamedLevelBeLoaded("Game"))
        {
            Assert.Fail("\"Game\" scene is misspelled or was not added to build settings");
        }

        PMHelper.TurnCollisions(false);
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        canvas = GameObject.Find("Canvas");
        if (!canvas)
        {
            Assert.Fail("There is no canvas in scene named \"Canvas\"");
        }

        score = GameObject.Find("Score");
        if (!score)
        {
            Assert.Fail("There is no score text-field in scene named \"Score\"");
        }

        if (!PMHelper.Exist<Canvas>(canvas))
            Assert.Fail("Canvas has no <Canvas> component");
        if (!PMHelper.Exist<CanvasScaler>(canvas))
            Assert.Fail("Canvas has no <Canvas Scaler> component");
        if (!PMHelper.Exist<GraphicRaycaster>(canvas))
            Assert.Fail("Canvas has no <Graphic Raycaster> component");
        if (!PMHelper.Exist<CanvasRenderer>(score))
            Assert.Fail("Score field has no <Canvas Renderer> component");
        if (!PMHelper.Exist<Text>(score))
            Assert.Fail("Score field has no <Text> component");

        if (!PMHelper.Child(score, canvas))
        {
            Assert.Fail("\"Score\" object should be a child of \"Canvas\" object");
        }

        RectTransform rect = PMHelper.Exist<RectTransform>(score);
        if (!PMHelper.CheckRectTransform(rect))
        {
            Assert.Fail("Anchors of \"Score\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }

        Text text = PMHelper.Exist<Text>(score);
        if (!text.text.Equals("0"))
        {
            Assert.Fail("\"Score\"'s text value should be initialized as \"0\" by default");
        }

        GameObject enemy = GameObject.FindWithTag("Enemy");

        EditorWindow game = null;
        double X, Y;
        (game, X, Y) = PMHelper.GetCoordinatesOnGameWindow(0.75f, 0.75f);

        VInput.MoveMouseTo(X, Y);
        VInput.LeftButtonClick();
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Bullet") || (Time.unscaledTime - start) * Time.timeScale > 2);
        GameObject bullet = GameObject.FindWithTag("Bullet");
        if (!bullet)
        {
            Assert.Fail("Bullet is not been spawned, or it's tag is misspelled");
        }

        GameObject bullet2 = GameObject.Instantiate(bullet);

        //Check score increasing 
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Test"), LayerMask.NameToLayer("Test"), false);
        bullet.layer = LayerMask.NameToLayer("Test");
        enemy.layer = LayerMask.NameToLayer("Test");
        bullet.transform.position = enemy.transform.position;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !bullet && !enemy || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (bullet || enemy)
        {
            Assert.Fail("Collision between bullets and enemies are not working properly");
        }

        int tmp = -1;
        try
        {
            tmp = int.Parse(text.text);
        }
        catch (Exception)
        {
            Assert.Fail("Score-text should always contain only integer value");
        }

        if (tmp <= 0)
        {
            Assert.Fail("Score should increase after destroying an enemy");
        }

        //Wait until strong enemies
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindGameObjectsWithTag("Enemy").Length == 10 ||
            (Time.unscaledTime - start) * Time.timeScale > 30);
        if (GameObject.FindGameObjectsWithTag("Enemy").Length != 10)
        {
            Assert.Fail("Enemies not spawning each 2 seconds");
        }

        foreach (var en in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GameObject.Destroy(en);
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (GameObject.FindWithTag("Enemy"))
        {
            Assert.Fail("Unexpected bug :(");
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 3);
        GameObject enemy2 = GameObject.FindWithTag("Enemy");

        //Check score again
        bullet2.layer = LayerMask.NameToLayer("Test");
        enemy2.layer = LayerMask.NameToLayer("Test");
        bullet2.transform.position = enemy2.transform.position;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !bullet2 && !enemy2 || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (bullet2 || enemy2)
        {
            Assert.Fail("Collision between bullets and enemies are not working properly");
        }

        int tmp2 = -1;
        try
        {
            tmp2 = int.Parse(text.text);
        }
        catch (Exception)
        {
            Assert.Fail("Score-text should always contain only integer value");
        }

        if (tmp2 - tmp <= tmp)
        {
            Assert.Fail("Killing enemies with increased difficulty should give more points");
        }

        game.maximized = false;
    }
}
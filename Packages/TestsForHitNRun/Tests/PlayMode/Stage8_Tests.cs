using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Description("Get Out Of There, It's Gonna Blow!"), Category("8")]
public class Stage8_Tests
{
    public GameObject play, exit, highScore, canvas, camera;
    public RectTransform playRT, exitRT, highScoreRT;
    public Text text;

    [UnityTest, Order(0)]
    public IEnumerator Check()
    {
        PlayerPrefs.DeleteAll();
        Time.timeScale = 15;
        if (!Application.CanStreamedLevelBeLoaded("Main Menu"))
        {
            Assert.Fail("\"Main Menu\" scene is misspelled or was not added to build settings");
        }

        PMHelper.TurnCollisions(false);
        SceneManager.LoadScene("Main Menu");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Main Menu" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            Assert.Fail("\"Main Menu\" scene can't be loaded");
        }

        // Check Objects
        canvas = GameObject.Find("Canvas");
        play = GameObject.Find("Play");
        exit = GameObject.Find("Exit");
        highScore = GameObject.Find("HighScore");
        camera = GameObject.Find("Main Camera");

        if (!canvas)
            Assert.Fail("There is no canvas in scene named \"Canvas\"");
        if (!play)
            Assert.Fail("There is no \"Play\" object in scene, or it is misspelled");
        if (!exit)
            Assert.Fail("There is no \"Exit\" object in scene, or it is misspelled");
        if (!highScore)
            Assert.Fail("There is no \"HighScore\" object in scene, or it is misspelled");

        if (!PMHelper.Exist<Canvas>(canvas))
            Assert.Fail("Canvas has no <Canvas> component");
        if (!PMHelper.Exist<CanvasScaler>(canvas))
            Assert.Fail("Canvas has no <Canvas Scaler> component");
        if (!PMHelper.Exist<GraphicRaycaster>(canvas))
            Assert.Fail("Canvas has no <Graphic Raycaster> component");

        playRT = PMHelper.Exist<RectTransform>(play);
        exitRT = PMHelper.Exist<RectTransform>(exit);
        highScoreRT = PMHelper.Exist<RectTransform>(highScore);

        if (!playRT)
            Assert.Fail("Play-button has no <Rect Transform> component");
        if (!PMHelper.Exist<CanvasRenderer>(play))
            Assert.Fail("Play-button has no <Canvas Renderer> component");
        if (!PMHelper.Exist<Button>(play))
            Assert.Fail("Play-button has no <Button> component");

        if (!exitRT)
            Assert.Fail("Exit-button has no <Rect Transform> component");
        if (!PMHelper.Exist<CanvasRenderer>(exit))
            Assert.Fail("Exit-button has no <Canvas Renderer> component");
        if (!PMHelper.Exist<Button>(exit))
            Assert.Fail("Exit-button has no <Button> component");

        text = PMHelper.Exist<Text>(highScore);
        if (!highScoreRT)
            Assert.Fail("HighScore text object has no <Rect Transform> component");
        if (!PMHelper.Exist<CanvasRenderer>(highScore))
            Assert.Fail("HighScore text object has no <Canvas Renderer> component");
        if (!text)
            Assert.Fail("HighScore text object has no <Text> component");
        if (!text.text.Equals("0"))
        {
            Assert.Fail("\"HighScore\"'s <Text> component's text value should be equal to \"0\" by default");
        }

        if (!PMHelper.Child(play, canvas))
            Assert.Fail("\"Play\" object should be a child of a \"Canvas\" object");
        if (!PMHelper.Child(exit, canvas))
            Assert.Fail("\"Exit\" object should be a child of a \"Canvas\" object");
        if (!PMHelper.Child(highScore, canvas))
            Assert.Fail("\"HighScore\" object should be a child of a \"Canvas\" object");

        if (!camera)
        {
            Assert.Fail("There is no camera object in \"Main Menu\" scene, named \"Main Camera\", or it is misspelled");
        }

        if (!PMHelper.Exist<Camera>(camera))
        {
            Assert.Fail("\"Main Camera\" object has no basic component <Camera>");
        }

        if (!PMHelper.CheckRectTransform(playRT))
        {
            Assert.Fail("Anchors of \"Play\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }

        if (!PMHelper.CheckRectTransform(exitRT))
        {
            Assert.Fail("Anchors of \"Exit\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }

        if (!PMHelper.CheckRectTransform(highScoreRT))
        {
            Assert.Fail("Anchors of \"HighScore\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }
    }

    [UnityTest, Order(3)]
    public IEnumerator CheckGameTab()
    {
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        Scene scene = SceneManager.GetActiveScene();
        VInput.KeyPress(KeyCode.Tab);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            scene != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (scene == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Tab-key is not providing scene changing from \"Game\" to \"Main Menu\"");
        }

        Scene mainMenu = SceneManager.GetActiveScene();

        if (!mainMenu.name.Equals("Main Menu"))
        {
            Assert.Fail("Pressing Tab-key is not providing scene changing from \"Game\" to \"Main Menu\"");
        }
    }

    [UnityTest, Order(4)]
    public IEnumerator CheckGameReload()
    {
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        Scene scene = SceneManager.GetActiveScene();
        VInput.KeyPress(KeyCode.R);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            scene != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (scene == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing R-key is not reloading \"Game\" scene");
        }

        Scene scene2 = SceneManager.GetActiveScene();

        if (!scene2.name.Equals("Game"))
        {
            Assert.Fail("Pressing R-key is not reloading \"Game\" scene");
        }
    }

    [UnityTest, Order(5)]
    public IEnumerator CheckMenuPlay()
    {
        if (!Application.CanStreamedLevelBeLoaded("Main Menu"))
        {
            Assert.Fail("\"Main Menu\" scene is misspelled or was not added to build settings");
        }

        SceneManager.LoadScene("Main Menu");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Main Menu" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            Assert.Fail("\"Main Menu\" scene can't be loaded");
        }

        Scene mainMenu = SceneManager.GetActiveScene();

        Button playB = PMHelper.Exist<Button>(GameObject.Find("Play"));
        playB.onClick.Invoke();

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            mainMenu != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (mainMenu == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Play-Button is not loading \"Game\" scene!");
        }

        Scene scene = SceneManager.GetActiveScene();

        if (!scene.name.Equals("Game"))
        {
            Assert.Fail("Pressing Play-Button is not loading \"Game\" scene!");
        }
    }

    [UnityTest, Order(6)]
    public IEnumerator CheckChangingPlayerPrefs()
    {
        //Deleting playerPrefs and loading Main Menu
        PlayerPrefs.DeleteAll();
        if (!Application.CanStreamedLevelBeLoaded("Main Menu"))
        {
            Assert.Fail("\"Main Menu\" scene is misspelled or was not added to build settings");
        }

        SceneManager.LoadScene("Main Menu");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Main Menu" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            Assert.Fail("\"Main Menu\" scene can't be loaded");
        }

        Scene mainMenu = SceneManager.GetActiveScene();

        //Checking if high score is 0
        text = PMHelper.Exist<Text>(GameObject.Find("HighScore"));
        if (!text.text.Equals("0"))
        {
            Assert.Fail("\"HighScore\"'s <Text> component's text value should be equal to \"0\" by default");
        }

        //Opening "Game" scene

        Button playB = PMHelper.Exist<Button>(GameObject.Find("Play"));
        playB.onClick.Invoke();

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            mainMenu != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (mainMenu == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Play-Button is not loading \"Game\" scene!");
        }

        //Getting objects
        GameObject enemy = GameObject.FindWithTag("Enemy");

        GameObject scoreObj = GameObject.Find("Score");
        Text scoreText = PMHelper.Exist<Text>(scoreObj);

        EditorWindow game;
        double X, Y;
        (game, X, Y) = PMHelper.GetCoordinatesOnGameWindow(0.75f, 0.75f);

        VInput.MoveMouseTo(X, Y);
        VInput.LeftButtonClick();
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Bullet") || (Time.unscaledTime - start) * Time.timeScale > 2);
        GameObject bullet = GameObject.FindWithTag("Bullet");
        GameObject bullet2 = GameObject.Instantiate(bullet);

        //Destroying two enemies
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

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 3);
        enemy = GameObject.FindWithTag("Enemy");

        bullet2.layer = LayerMask.NameToLayer("Test");
        enemy.layer = LayerMask.NameToLayer("Test");
        bullet2.transform.position = enemy.transform.position;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !bullet2 && !enemy || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (bullet2 || enemy)
        {
            Assert.Fail("Collision between bullets and enemies are not working properly");
        }

        //Getting score and loading Main Menu
        int score = -1;
        try
        {
            score = int.Parse(scoreText.text);
        }
        catch (Exception)
        {
            Assert.Fail("Score-text should always contain only integer value");
        }

        Scene scene = SceneManager.GetActiveScene();
        VInput.KeyPress(KeyCode.Tab);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            scene != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (scene == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Tab-key is not providing scene changing from \"Game\" to \"Main Menu\"");
        }

        highScore = GameObject.Find("HighScore");
        text = PMHelper.Exist<Text>(highScore);

        //Getting high score and checking if it is the score, we've earned
        int high = -1;
        try
        {
            high = int.Parse(text.text);
        }
        catch (Exception)
        {
            Assert.Fail("HighScore-text should always contain only integer value");
        }

        if (high != score)
        {
            Assert.Fail("High score is not being updated, or is being updated incorrectly," +
                        " when player gets more points");
        }

        //Loading "Game" again and setting up the scene
        playB = PMHelper.Exist<Button>(GameObject.Find("Play"));
        playB.onClick.Invoke();

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            mainMenu != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (mainMenu == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Play-Button is not loading \"Game\" scene!");
        }

        //Getting objects
        enemy = GameObject.FindWithTag("Enemy");

        (game, X, Y) = PMHelper.GetCoordinatesOnGameWindow(0.75f, 0.75f);

        VInput.MoveMouseTo(X, Y);
        VInput.LeftButtonClick();
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            GameObject.FindWithTag("Bullet") || (Time.unscaledTime - start) * Time.timeScale > 2);
        bullet = GameObject.FindWithTag("Bullet");

        //Destroying only one enemy
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


        //Loading "Main Menu" and checking if new highscore is not updated
        scene = SceneManager.GetActiveScene();
        VInput.KeyPress(KeyCode.Tab);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            scene != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 5);
        if (scene == SceneManager.GetActiveScene())
        {
            Assert.Fail("Pressing Tab-key is not providing scene changing from \"Game\" to \"Main Menu\"");
        }

        highScore = GameObject.Find("HighScore");
        text = PMHelper.Exist<Text>(highScore);

        int cur = -1;
        try
        {
            cur = int.Parse(text.text);
        }
        catch (Exception)
        {
            Assert.Fail("HighScore-text should always contain only integer value");
        }

        if (cur != score)
        {
            Assert.Fail("High score should change only when player gets more points");
        }

        game.maximized = false;
    }
}
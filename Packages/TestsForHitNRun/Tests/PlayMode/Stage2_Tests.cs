using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

[Description("Come on, come on. Chop-chop!"), Category("2")]
public class Stage2_Tests
{
    public GameObject player;
    public Camera camera;
    public GameObject cameraObj;

    [UnityTest, Order(1)]
    public IEnumerator SetUp()
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

        player = GameObject.Find("Player");
    }

    [UnityTest, Order(2)]
    public IEnumerator CheckLeftMovement()
    {
        Vector3 startpos = player.transform.position;

        VInput.KeyDown(KeyCode.A);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            player.transform.position != startpos || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (player.transform.position == startpos)
        {
            Assert.Fail("By pressing \"A\"-key, player should move to the left, x-axis should decrease");
        }

        VInput.KeyUp(KeyCode.A);

        Vector3 endpos = player.transform.position;
        if (endpos.x >= startpos.x)
        {
            Assert.Fail("By pressing \"A\"-key, player should move to the left, x-axis should decrease");
        }

        if (endpos.y != startpos.y || endpos.z != startpos.z)
        {
            Assert.Fail("By moving left (\"A\"-key) y-axis or z-axis should not change");
        }
    }

    [UnityTest, Order(3)]
    public IEnumerator CheckRightMovement()
    {
        Vector3 startpos = player.transform.position;

        VInput.KeyDown(KeyCode.D);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            player.transform.position != startpos || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (player.transform.position == startpos)
        {
            Assert.Fail("By pressing \"D\"-key, player should move to the right, x-axis should increase");
        }

        VInput.KeyUp(KeyCode.D);

        Vector3 endpos = player.transform.position;
        if (endpos.x <= startpos.x)
        {
            Assert.Fail("By pressing \"D\"-key, player should move to the right, x-axis should increase");
        }

        if (endpos.y != startpos.y || endpos.z != startpos.z)
        {
            Assert.Fail("By moving left (\"D\"-key) y-axis or z-axis should not change");
        }
    }

    [UnityTest, Order(4)]
    public IEnumerator CheckDownMovement()
    {
        Vector3 startpos = player.transform.position;

        VInput.KeyDown(KeyCode.S);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            player.transform.position != startpos || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (player.transform.position == startpos)
        {
            Assert.Fail("By pressing \"S\"-key, player should move down, y-axis should decrease");
        }

        VInput.KeyUp(KeyCode.S);

        Vector3 endpos = player.transform.position;
        if (endpos.y >= startpos.y)
        {
            Assert.Fail("By pressing \"S\"-key, player should move down, y-axis should decrease");
        }

        if (endpos.x != startpos.x || endpos.z != startpos.z)
        {
            Assert.Fail("By moving down x-axis or z-axis should not change");
        }
    }

    [UnityTest, Order(5)]
    public IEnumerator CheckUpMovement()
    {
        yield return null;
        Vector3 startpos = player.transform.position;

        VInput.KeyDown(KeyCode.W);
        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            player.transform.position != startpos || (Time.unscaledTime - start) * Time.timeScale > 2);
        if (player.transform.position == startpos)
        {
            Assert.Fail("By pressing \"W\"-key, player should move up, y-axis should increase");
        }

        VInput.KeyUp(KeyCode.W);

        Vector3 endpos = player.transform.position;
        if (endpos.y <= startpos.y)
        {
            Assert.Fail("By pressing \"W\"-key, player should move up, y-axis should increase");
        }

        if (endpos.x != startpos.x || endpos.z != startpos.z)
        {
            Assert.Fail("By moving up x-axis or z-axis should not change");
        }
    }

    [UnityTest, Order(6)]
    public IEnumerator CameraFollowCheck()
    {
        player = GameObject.Find("Player");
        cameraObj = GameObject.Find("Main Camera");
        camera = PMHelper.Exist<Camera>(cameraObj);
        KeyCode[] ways = {KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W};
        foreach (KeyCode w in ways)
        {
            VInput.KeyDown(w);
            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !PMHelper.CheckVisibility(camera, player.transform, 2) ||
                (Time.unscaledTime - start) * Time.timeScale > 5);
            if (!PMHelper.CheckVisibility(camera, player.transform, 2))
            {
                Assert.Fail("Camera should follow the \"Player\" object");
            }

            VInput.KeyUp(w);
        }
    }

    [UnityTest, Order(7)]
    public IEnumerator LookingTest()
    {
        yield return null;
        player = GameObject.Find("Player");
        Transform shotgunT = GameObject.Find("Shotgun").GetComponent<Transform>();
        cameraObj = GameObject.Find("Main Camera");
        camera = PMHelper.Exist<Camera>(cameraObj);
        Vector2 baseMouse = Input.mousePosition;

        EditorWindow game = null;
        double X, Y;
        for (int i = 0; i < 10; i++)
        {
            (game, X, Y) = PMHelper.GetCoordinatesOnGameWindow(Random.Range(0f, 1f), Random.Range(0f, 1f));
            VInput.MoveMouseTo(X, Y);
            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !baseMouse.Equals(Input.mousePosition) || (Time.unscaledTime - start) * Time.timeScale > 2);
            if (baseMouse.Equals(Input.mousePosition))
            {
                Assert.Fail(
                    "Player rotation is not working properly, it's shotgun should be facing the mouse cursor directly");
            }

            Vector2 mouse = camera.ScreenToWorldPoint(Input.mousePosition);
            float startDist = Vector2.Distance(shotgunT.position, mouse);
            player.transform.Rotate(Vector3.forward, 10);
            float endDist1 = Vector2.Distance(shotgunT.position, mouse);
            player.transform.Rotate(Vector3.forward, -20);
            float endDist2 = Vector2.Distance(shotgunT.position, mouse);
            if (startDist >= endDist1 || startDist >= endDist2)
            {
                Assert.Fail(
                    "Player rotation is not working properly, it's shotgun should be facing the mouse cursor directly");
            }

            baseMouse = Input.mousePosition;
        }

        game.maximized = false;
    }
}
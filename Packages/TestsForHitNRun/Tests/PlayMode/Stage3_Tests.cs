using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("We're in the clear."), Category("3")]
public class Stage3_Tests
{
    private GameObject player;
    private List<Collider2D> borderColls = new List<Collider2D>();

    [UnityTest]
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

        //Check tag existance

        if (!PMHelper.CheckTagExistance("Border"))
        {
            Assert.Fail("\"Border\" tag was not added to project");
        }

        //Check if there are borders
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
        if (borders.Length != 4)
        {
            Assert.Fail("There should be 4 borders with \"Border\" tag");
        }

        foreach (GameObject b in borders)
        {
            Collider2D coll = PMHelper.Exist<Collider2D>(b);
            SpriteRenderer srBorder = PMHelper.Exist<SpriteRenderer>(b);
            if (!srBorder || !srBorder.enabled)
            {
                Assert.Fail("There is no <SpriteRenderer> component on \"Border\" object or it is disabled!");
            }

            if (!srBorder.sprite)
            {
                Assert.Fail("There is no sprite assigned to \"Border\"'s <SpriteRenderer>!");
            }

            if (!coll)
            {
                Assert.Fail("Each \"Border\" object should have assigned <Collider2D> component");
            }

            if (coll.isTrigger)
            {
                Assert.Fail("Each \"Border\" object's <Collider2D> component should not be triggerable");
            }

            borderColls.Add(coll);
            b.layer = LayerMask.NameToLayer("Test");
        }

        player = GameObject.Find("Player");
        Transform playerT = PMHelper.Exist<Transform>(player);

        int N = 36;
        for (int i = 0; i < N; i++)
        {
            playerT.Rotate(0, 0, 360 / N);
            if (!PMHelper.RaycastFront2D(playerT.position, playerT.up,
                1 << LayerMask.NameToLayer("Test")).collider)
            {
                Assert.Fail("Player should be surrounded by borders");
            }
        }

        //Check player's components
        Collider2D playerColl = PMHelper.Exist<Collider2D>(player);
        if (!playerColl)
        {
            Assert.Fail("Player should have assigned <Collider2D> component");
        }

        if (playerColl.isTrigger)
        {
            Assert.Fail("Player's <Collider2D> component should not be triggerable");
        }

        Rigidbody2D playerRb = PMHelper.Exist<Rigidbody2D>(player);
        if (!playerRb)
        {
            Assert.Fail("Player should have assigned <Rigidbody2D> component");
        }

        if (playerRb.bodyType != RigidbodyType2D.Dynamic)
        {
            Assert.Fail("Player's <Rigidbody2D> component should be Dynamic");
        }

        if (!playerRb.simulated)
        {
            Assert.Fail("Player's <Rigidbody2D> component should be simulated");
        }

        if (playerRb.gravityScale != 0)
        {
            Assert.Fail("Player's <Rigidbody2D> component should not be affected by gravity, " +
                        "so it's Gravity Scale parameter should be equal to 0");
        }

        if (playerRb.interpolation != RigidbodyInterpolation2D.None)
        {
            Assert.Fail("Do not change interpolation of Player's <Rigidbody2D> component. Set it as None");
        }

        if (playerRb.constraints != RigidbodyConstraints2D.None)
        {
            Assert.Fail("Do not freeze any Player's <Rigidbody2D> component's constraints");
        }

        //Check movement with freezed rigidbody
        Vector3 startP = playerT.position;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        KeyCode[] ways = {KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W};
        foreach (KeyCode w in ways)
        {
            VInput.KeyDown(w);
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                playerT.position != startP || (Time.unscaledTime - start) * Time.timeScale > 2);
            if (playerT.position != startP)
            {
                Assert.Fail("Player's movement should be reimplemented with <Rigidbody2D> component usage");
            }

            VInput.KeyUp(w);
        }

        playerRb.constraints = RigidbodyConstraints2D.None;

        //Check FixedUpdate() method usage        
        Vector3 bef = playerT.position;
        foreach (KeyCode w in ways)
        {
            VInput.KeyDown(w);
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                playerT.position != bef || (Time.unscaledTime - start) * Time.timeScale > 2);

            for (int i = 0; i < 10; i++)
            {
                bef = playerT.position;
                yield return new WaitForFixedUpdate();
                if (bef.Equals(playerT.position))
                {
                    Assert.Fail("Use FixedUpdate() event for player's physics simulation");
                }
            }

            VInput.KeyUp(w);
        }

        bool Collides(Collider2D m, List<Collider2D> list)
        {
            return m.IsTouching(list[0]) ||
                   m.IsTouching(list[1]) ||
                   m.IsTouching(list[2]) ||
                   m.IsTouching(list[3]);
        }

        //Check bounds
        int basicPlayerLayer = player.layer;
        player.layer = LayerMask.NameToLayer("Test");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Test"), LayerMask.NameToLayer("Test"), false);
        playerT.position = startP;
        foreach (KeyCode w in ways)
        {
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !Collides(playerColl, borderColls) || (Time.unscaledTime - start) * Time.timeScale > 1);

            if (Collides(playerColl, borderColls))
            {
                Assert.Fail("Player object should not be placed right next to bounds");
            }

            VInput.KeyDown(w);
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                Collides(playerColl, borderColls) || (Time.unscaledTime - start) * Time.timeScale > 6);

            if (!Collides(playerColl, borderColls))
            {
                Assert.Fail("There are no bounds of field for the player, or they are placed too far" +
                            " (Player should be able to reach each border from it's start position in less than 5 seconds)");
            }

            VInput.KeyUp(w);

            playerT.position = startP;
        }

        player.layer = basicPlayerLayer;
        PMHelper.TurnCollisions(false);

        //Checking obstacles

        if (!PMHelper.CheckTagExistance("Obstacle"))
        {
            Assert.Fail("\"Obstacle\" tag was not added to project");
        }

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length != 5)
        {
            Assert.Fail("There should be at least 5 obstacles with \"Obstacle\" tag");
        }

        playerT.position = startP;
        float left = PMHelper.RaycastFront2D(playerT.position, Vector2.left,
            1 << LayerMask.NameToLayer("Test")).point.x;
        float right = PMHelper.RaycastFront2D(playerT.position, Vector2.right,
            1 << LayerMask.NameToLayer("Test")).point.x;
        float up = PMHelper.RaycastFront2D(playerT.position, Vector2.up,
            1 << LayerMask.NameToLayer("Test")).point.y;
        float down = PMHelper.RaycastFront2D(playerT.position, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).point.y;

        foreach (GameObject o in obstacles)
        {
            SpriteRenderer sr = PMHelper.Exist<SpriteRenderer>(o);
            if (!sr || !sr.enabled)
            {
                Assert.Fail("There is no <SpriteRenderer> component on \"Obstacle\" object or it is disabled!");
            }

            if (!sr.sprite)
            {
                Assert.Fail("There is no sprite assigned to \"Obstacle\"'s <SpriteRenderer>!");
            }

            Color playerColor = GameObject.Find("Player").GetComponent<SpriteRenderer>().color;
            Color shotgunColor = GameObject.Find("Shotgun").GetComponent<SpriteRenderer>().color;
            Color backColor = GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor;
            Color obstacleColor = sr.color;

            if (!PMHelper.CheckColorDifference(playerColor, obstacleColor, 0.3f))
            {
                Assert.Fail("The difference of colors between \"Player\" and obstacles should be visible!");
            }

            if (!PMHelper.CheckColorDifference(shotgunColor, obstacleColor, 0.3f))
            {
                Assert.Fail("The difference of colors between \"Shotgun\" and obstacles should be visible!");
            }

            if (!PMHelper.CheckColorDifference(backColor, obstacleColor, 0.3f))
            {
                Assert.Fail("The difference of colors between background and obstacles should be visible!");
            }

            Transform oT = PMHelper.Exist<Transform>(o);
            if (!PMHelper.CheckObjectFits2D(oT, new Vector2(left, up), new Vector2(right, down)))
            {
                Assert.Fail("Obstacles should be inside limiting zone");
            }

            Collider2D coll = PMHelper.Exist<Collider2D>(o);
            if (!coll)
            {
                Assert.Fail("Obstacles should have assigned <Collider2D> component");
            }

            if (coll.isTrigger)
            {
                Assert.Fail("Obstacles' <Collider2D> component should not be triggerable");
            }
        }
    }
}
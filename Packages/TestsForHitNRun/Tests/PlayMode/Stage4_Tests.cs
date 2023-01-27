using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("Targets acquired."), Category("4")]
public class Stage4_Tests
{
    private GameObject player;
    private float left, right, up, down;
    List<Vector2> enemies = new List<Vector2>();

    [UnityTest, Order(0)]
    public IEnumerator CheckEnemiesAndSpawn()
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
        //Check Enemy
        if (!PMHelper.CheckTagExistance("Enemy"))
        {
            Assert.Fail("\"Enemy\" tag was not added to project");
        }

        //Checking Enemy's parameters
        GameObject tmp = GameObject.FindWithTag("Enemy");
        if (!tmp)
        {
            Assert.Fail("Enemies not spawning instantly after opening scene, or prefab's tag is misspelled");
        }

        SpriteRenderer srEnemy = PMHelper.Exist<SpriteRenderer>(tmp);
        if (!srEnemy || !srEnemy.enabled)
        {
            Assert.Fail("There is no <SpriteRenderer> component on \"Enemy\" object or it is disabled");
        }

        if (!srEnemy.sprite)
        {
            Assert.Fail("There is no sprite assigned to \"Enemy\"'s <SpriteRenderer>");
        }

        Collider2D collEnemy = PMHelper.Exist<Collider2D>(tmp);
        if (!collEnemy)
        {
            Assert.Fail("\"Enemy\" objects' should have assigned <Collider2D> component");
        }

        if (collEnemy.isTrigger)
        {
            Assert.Fail("\"Enemy\" objects' <Collider2D> component should not be triggerable");
        }

        Color enemyColor = srEnemy.color;
        Color playerColor = player.GetComponent<SpriteRenderer>().color;
        Color shotgunColor = PMHelper.Exist<SpriteRenderer>(GameObject.Find("Shotgun")).color;
        Color backColor = PMHelper.Exist<Camera>(GameObject.Find("Main Camera")).backgroundColor;
        Color obstacleColor = PMHelper.Exist<SpriteRenderer>(GameObject.FindWithTag("Obstacle")).color;

        if (!PMHelper.CheckColorDifference(enemyColor, playerColor, 0.3f))
            Assert.Fail("The difference of colors between \"Player\" and enemies should be visible");
        if (!PMHelper.CheckColorDifference(enemyColor, backColor, 0.3f))
            Assert.Fail("The difference of colors between enemies and background should be visible");
        if (!PMHelper.CheckColorDifference(enemyColor, shotgunColor, 0.3f))
            Assert.Fail("The difference of colors between \"Shotgun\" and enemies should be visible");
        if (!PMHelper.CheckColorDifference(enemyColor, obstacleColor, 0.3f))
            Assert.Fail("The difference of colors between enemies and obstacles should be visible");

        Rigidbody2D enemyRb = PMHelper.Exist<Rigidbody2D>(tmp);
        if (!enemyRb)
        {
            Assert.Fail("Enemies should have assigned <Rigidbody2D> component");
        }

        if (enemyRb.bodyType != RigidbodyType2D.Dynamic)
        {
            Assert.Fail("Enemies' <Rigidbody2D> component should be Dynamic");
        }

        if (!enemyRb.simulated)
        {
            Assert.Fail("Enemies' <Rigidbody2D> component should be simulated");
        }

        if (enemyRb.gravityScale != 0)
        {
            Assert.Fail("Enemies' <Rigidbody2D> component should not be affected by gravity, " +
                        "so it's Gravity Scale parameter should be equal to 0");
        }

        if (enemyRb.interpolation != RigidbodyInterpolation2D.None)
        {
            Assert.Fail("Do not change interpolation of Enemies' <Rigidbody2D> component. Set it as None");
        }

        if (enemyRb.constraints != RigidbodyConstraints2D.None)
        {
            Assert.Fail("Do not freeze any Enemies' <Rigidbody2D> component's constraints");
        }

        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Border"))
        {
            b.layer = LayerMask.NameToLayer("Test");
        }

        Vector3 playerPos = player.transform.position;
        left = PMHelper.RaycastFront2D(playerPos, Vector2.left,
            1 << LayerMask.NameToLayer("Test")).point.x;
        right = PMHelper.RaycastFront2D(playerPos, Vector2.right,
            1 << LayerMask.NameToLayer("Test")).point.x;
        up = PMHelper.RaycastFront2D(playerPos, Vector2.up,
            1 << LayerMask.NameToLayer("Test")).point.y;
        down = PMHelper.RaycastFront2D(playerPos, Vector2.down,
            1 << LayerMask.NameToLayer("Test")).point.y;

        //Check Enemy Spawn
        GameObject.Destroy(tmp);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (GameObject.FindWithTag("Enemy"))
        {
            Assert.Fail("There should be spawned only one enemy each 2 seconds");
        }

        Time.timeScale = 5;
        for (int j = 0; j < 10; j++)
        {
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 3);
            GameObject tmp2 = GameObject.FindWithTag("Enemy");
            if (!tmp2)
            {
                Assert.Fail("Enemies not spawning each 2 seconds");
            }

            if (!PMHelper.CheckObjectFits2D(tmp2.transform, new Vector2(left, up), new Vector2(right, down)))
            {
                Assert.Fail("Enemies should be instantiated inside limited area");
            }

            enemies.Add(tmp2.transform.position);

            if (PMHelper.CheckVisibility(Camera.main, tmp2.transform, 2))
            {
                Assert.Fail("Enemies should not be visible when spawned (should be instantiated off the camera view)" +
                            " Make sure, that enemies are not too fast to enter camera view at their first frame of life");
            }

            GameObject.Destroy(GameObject.FindWithTag("Enemy"));
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !GameObject.FindWithTag("Enemy") || (Time.unscaledTime - start) * Time.timeScale > 1);
            if (GameObject.FindWithTag("Enemy"))
            {
                Assert.Fail("There should be spawned only one enemy each 2 seconds");
            }
        }

        if (enemies.Count != enemies.Distinct().Count())
        {
            Assert.Fail("Enemies should be spawned randomly");
        }
    }

    [UnityTest, Order(1)]
    public IEnumerator CheckEnemyMovement()
    {
        Time.timeScale = 15;
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        Transform playerT = GameObject.Find("Player").transform;
        Transform enemyT = GameObject.FindWithTag("Enemy").transform;

        enemyT.position = new Vector3(left + (right - left) / 4, down + (up - down) / 4);
        playerT.position = new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4 * 3);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerT.position == new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4 * 3) ||
            (Time.unscaledTime - start) * Time.timeScale > 1);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(enemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 1);
        if (Vector2.Distance(enemyT.position, playerT.position) < 0.01f)
        {
            Assert.Fail("Enemies are moving to a player too fast");
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(enemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 20);
        if (Vector2.Distance(enemyT.position, playerT.position) >= 0.01f)
        {
            Assert.Fail("Enemies are not moving to a player, or moving too slow");
        }

        playerT.position = new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerT.position == new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4) ||
            (Time.unscaledTime - start) * Time.timeScale > 1);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(enemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 1);
        if (Vector2.Distance(enemyT.position, playerT.position) < 0.01f)
        {
            Assert.Fail("Enemies are moving to a player too fast");
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(enemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 20);
        if (Vector2.Distance(enemyT.position, playerT.position) >= 0.01f)
        {
            Assert.Fail("Enemies are not moving to a player, or moving too slow");
        }

        //Check movement via rigidbody
        Rigidbody2D enemyRb = PMHelper.Exist<Rigidbody2D>(enemyT.gameObject);
        enemyRb.constraints = RigidbodyConstraints2D.FreezeAll;

        Vector3 bef = enemyT.position;
        playerT.position = new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4 * 3);
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            playerT.position == new Vector3(left + (right - left) / 4 * 3, down + (up - down) / 4 * 3) ||
            (Time.unscaledTime - start) * Time.timeScale > 1);

        if (bef != enemyT.position)
        {
            Assert.Fail("Enemies' movement was not implemented with <Rigidbody2D> component usage");
        }

        enemyRb.constraints = RigidbodyConstraints2D.None;

        //Check FixedUpdate() method usage        
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
            if (enemyT.position.Equals(bef))
            {
                Assert.Fail("Use FixedUpdate() event for enemy's physics simulation");
            }

            bef = enemyT.position;
        }
    }

    [UnityTest, Order(2)]
    public IEnumerator CheckCollisions()
    {
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        Scene cur = SceneManager.GetActiveScene();

        player = GameObject.Find("Player");
        GameObject enemy = GameObject.FindWithTag("Enemy");
        GameObject obstacle = GameObject.FindWithTag("Obstacle");
        GameObject border = GameObject.FindWithTag("Border");

        Vector3 playerPos = GameObject.Find("Player").transform.position;

        enemy.transform.position = playerPos;
        obstacle.transform.position = playerPos;
        border.transform.position = playerPos;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Test"), LayerMask.NameToLayer("Test"), false);

        (GameObject, GameObject, string, string)[] listOfTests =
        {
            (player, border, "player", "borders"),
            (enemy, border, "enemies", "borders"),
            (obstacle, border, "obstacles", "borders"),
            (player, obstacle, "player", "obstacles"),
            (enemy, obstacle, "enemies", "obstacles"),
        };

        foreach (var testCase in listOfTests)
        {
            LayerMask layer1 = testCase.Item1.layer, layer2 = testCase.Item2.layer;
            testCase.Item1.layer = LayerMask.NameToLayer("Test");
            testCase.Item2.layer = LayerMask.NameToLayer("Test");

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                cur != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 1);
            if (cur != SceneManager.GetActiveScene())
            {
                Assert.Fail("When " + testCase.Item3 + " collide with " + testCase.Item4 + " scene should not reload");
            }

            testCase.Item1.layer = layer1;
            testCase.Item2.layer = layer2;

            player.transform.position = playerPos;
            enemy.transform.position = playerPos;
            obstacle.transform.position = playerPos;
            border.transform.position = playerPos;

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                testCase.Item1.layer == layer1 && testCase.Item2.layer == layer2 ||
                (Time.unscaledTime - start) * Time.timeScale > 3);
            if (testCase.Item1.layer != layer1 || testCase.Item2.layer != layer2)
            {
                Assert.Fail("Unexpected");
            }
        }

        player.layer = LayerMask.NameToLayer("Test");
        enemy.layer = LayerMask.NameToLayer("Test");

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            cur != SceneManager.GetActiveScene() || (Time.unscaledTime - start) * Time.timeScale > 3);
        if (cur == SceneManager.GetActiveScene())
        {
            Assert.Fail("When player collide with enemies scene should reload");
        }
    }
}
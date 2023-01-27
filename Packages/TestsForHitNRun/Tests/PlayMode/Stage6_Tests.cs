using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

[Description("Remember! This is bandit country."), Category("6")]
public class Stage6_Tests
{
    [UnityTest, Order(0)]
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
    }

    [UnityTest, Order(1)]
    public IEnumerator CheckSpeedAndColorIncrease()
    {
        GameObject firstEnemy = GameObject.FindWithTag("Enemy");
        Transform firstEnemyT = firstEnemy.transform;
        Vector3 point = firstEnemyT.position;
        Color first = PMHelper.Exist<SpriteRenderer>(firstEnemy).color;

        GameObject player = GameObject.Find("Player");
        Transform playerT = player.transform;

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(firstEnemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 20);
        if (Vector2.Distance(firstEnemyT.position, playerT.position) >= 0.01f)
        {
            Assert.Fail("Enemies are not moving to a player, or moving too slow");
        }

        float firstTime = Time.unscaledTime - start;

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
        GameObject secondEnemy = GameObject.FindWithTag("Enemy");
        if (!secondEnemy)
        {
            Assert.Fail("Enemies not spawning each 2 seconds");
        }

        Transform secondEnemyT = secondEnemy.transform;
        Color second = PMHelper.Exist<SpriteRenderer>(secondEnemy).color;

        secondEnemyT.position = point;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            secondEnemyT.position == point || (Time.unscaledTime - start) * Time.timeScale > 1);

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Vector2.Distance(secondEnemyT.position, playerT.position) < 0.01f ||
            (Time.unscaledTime - start) * Time.timeScale > 20);
        if (Vector2.Distance(secondEnemyT.position, playerT.position) >= 0.01f)
        {
            Assert.Fail("Enemies are not moving to a player, or moving too slow");
        }

        float secondTime = Time.unscaledTime - start;

        if (first == second)
        {
            Assert.Fail("After 20+ seconds of game-time, color of new enemies didn't change");
        }

        if (secondTime >= firstTime)
        {
            Assert.Fail("After 20+ seconds of game-time, speed of new enemies didn't increase");
        }
    }
}
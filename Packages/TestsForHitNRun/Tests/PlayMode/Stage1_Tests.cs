using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("Gear up! We're going in!"), Category("1")]
public class Stage1_Tests
{
    private GameObject player, shotgun;
    private GameObject camera;
    private Camera cameraComp;
    private bool exist;
    private SpriteRenderer playerSR, shotgunSR;

    [UnityTest, Order(1)]
    public IEnumerator CheckPlayerObjects()
    {
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

        if (!PMHelper.CheckLayerExistance("Test"))
        {
            Assert.Fail("Please, do not remove \"Test\" layer, it's existence necessary for tests");
        }

        (player, exist) = PMHelper.Exist("Player");
        if (!exist)
        {
            Assert.Fail("There is no object \"Player\" in scene, or it is misspelled");
        }

        (shotgun, exist) = PMHelper.Exist("Shotgun");
        if (!exist)
        {
            Assert.Fail("There is no object \"Shotgun\" in scene, or it is misspelled");
        }

        if (!PMHelper.Child(shotgun, player))
        {
            Assert.Fail("Object \"Shotgun\" is not a child of \"Player\" object");
        }

        (camera, exist) = PMHelper.Exist("Main Camera");

        if (!camera)
        {
            Assert.Fail("There is no camera object in scene, named \"Main Camera\", or it is misspelled");
        }

        cameraComp = PMHelper.Exist<Camera>(camera);

        if (!cameraComp)
        {
            Assert.Fail("\"Main Camera\" object has no basic component <Camera>");
        }

        playerSR = PMHelper.Exist<SpriteRenderer>(player);
        shotgunSR = PMHelper.Exist<SpriteRenderer>(shotgun);
        if (!playerSR || !playerSR.enabled)
        {
            Assert.Fail("There is no <SpriteRenderer> component on \"Player\" object or it is disabled!");
        }

        if (!playerSR.sprite)
        {
            Assert.Fail("There is no sprite assigned to \"Player\"'s <SpriteRenderer>!");
        }

        if (!shotgunSR || !shotgunSR.enabled)
        {
            Assert.Fail("There is no <SpriteRenderer> component on \"Shotgun\" object or it is disabled!");
        }

        if (!shotgunSR.sprite)
        {
            Assert.Fail("There is no sprite assigned to \"Shotgun\"'s <SpriteRenderer>!");
        }

        if (!PMHelper.CheckColorDifference(playerSR.color, shotgunSR.color, 0.3f))
        {
            Assert.Fail("The difference of colors between \"Player\" and \"Shotgun\" should be visible!");
        }

        if (!PMHelper.CheckColorDifference(playerSR.color, cameraComp.backgroundColor, 0.3f))
        {
            Assert.Fail("The difference of colors between \"Player\" and \"Camera\"'s background" +
                        "should be visible!");
        }

        if (!PMHelper.CheckColorDifference(shotgunSR.color, cameraComp.backgroundColor, 0.3f))
        {
            Assert.Fail("The difference of colors between \"Shotgun\" and \"Camera\"'s background" +
                        "should be visible!");
        }

        if (!PMHelper.CheckVisibility(cameraComp, player.transform, 2))
        {
            Assert.Fail("\"Player\" object is not visible by camera");
        }

        if (!PMHelper.CheckVisibility(cameraComp, shotgun.transform, 2))
        {
            Assert.Fail("\"Shotgun\" object is not visible by camera");
        }

        if (playerSR.sortingLayerID != shotgunSR.sortingLayerID)
        {
            Assert.Fail("You don't need to change the \"Sorting Layer\" parameter in <SpriteRenderer> component," +
                        "in order to change order of rendering. Leave objects on the same sorting layer and change the" +
                        "\"Order in Layer\" parameter");
        }

        if (playerSR.sortingOrder <= shotgunSR.sortingOrder)
        {
            Assert.Fail(
                "Player should be visible in front of shotgun, so player's order in layer should be greater than shotgun's one");
        }

        Transform shotgunT = shotgun.transform, playerT = player.transform;

        if (shotgunT.localPosition == Vector3.zero)
        {
            Assert.Fail("Shotgun should not be placed at center of Player");
        }

        float shotgunX = shotgunT.localPosition.x;
        float shotgunY = shotgunT.localPosition.y;
        if (shotgunX != 0 && shotgunY != 0)
        {
            Assert.Fail("Shotgun's local position should have x-axis equal to zero, or y-axis position equal to zero" +
                        ", so that player is looking up/down/left/right");
        }

        String way = "";
        if (shotgunX == 0 && shotgunY > 0) way = "up";
        if (shotgunX == 0 && shotgunY < 0) way = "down";
        if (shotgunX < 0 && shotgunY == 0) way = "left";
        if (shotgunX > 0 && shotgunY == 0) way = "right";


        shotgunT.rotation = Quaternion.Euler(0, 0, 0);
        playerT.rotation = Quaternion.Euler(0, 0, 0);

        bool correct = true;
        switch (way)
        {
            case "up":
                correct = shotgunT.position.y - shotgunT.lossyScale.y / 2 <=
                          playerT.position.y + playerT.lossyScale.y / 2;
                break;
            case "down":
                correct = shotgunT.position.y + shotgunT.lossyScale.y / 2 >=
                          playerT.position.y - playerT.lossyScale.y / 2;
                break;
            case "right":
                correct = shotgunT.position.x - shotgunT.lossyScale.x / 2 <=
                          playerT.position.x + playerT.lossyScale.x / 2;
                break;
            case "left":
                correct = shotgunT.position.x + shotgunT.lossyScale.x / 2 >=
                          playerT.position.x - playerT.lossyScale.x / 2;
                break;
        }

        if (!correct)
        {
            Assert.Fail("Make sure, that there is no gap between player and shotgun.");
        }
    }
}
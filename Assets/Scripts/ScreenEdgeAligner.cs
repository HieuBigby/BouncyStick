using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEdgeAligner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer topSprite;
    [SerializeField] private SpriteRenderer leftSprite;
    [SerializeField] private SpriteRenderer rightSprite;
    [SerializeField] private float fadeDuration = 1.0f; // Duration of the fade effect

    private void Start()
    {
        // Set initial alpha values to 0 to hide the sprites
        SetSpriteAlpha(0f);
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void GameStarted()
    {
        AlignSpritesWithScreen();
        StartCoroutine(FadeInSprites());
    }

    private void OnGameEnded()
    {
        StartCoroutine(FadeOutSprites());
    }

    private IEnumerator FadeInSprites()
    {
        float elapsedTime = 0f;
        Color topColor = topSprite.color;
        Color leftColor = leftSprite.color;
        Color rightColor = rightSprite.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            topSprite.color = new Color(topColor.r, topColor.g, topColor.b, alpha);
            leftSprite.color = new Color(leftColor.r, leftColor.g, leftColor.b, alpha);
            rightSprite.color = new Color(rightColor.r, rightColor.g, rightColor.b, alpha);

            yield return null;
        }

        // Ensure the sprites are fully opaque at the end
        topSprite.color = new Color(topColor.r, topColor.g, topColor.b, 1f);
        leftSprite.color = new Color(leftColor.r, leftColor.g, leftColor.b, 1f);
        rightSprite.color = new Color(rightColor.r, rightColor.g, rightColor.b, 1f);
    }

    private IEnumerator FadeOutSprites()
    {
        float elapsedTime = 0f;
        Color topColor = topSprite.color;
        Color leftColor = leftSprite.color;
        Color rightColor = rightSprite.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            topSprite.color = new Color(topColor.r, topColor.g, topColor.b, alpha);
            leftSprite.color = new Color(leftColor.r, leftColor.g, leftColor.b, alpha);
            rightSprite.color = new Color(rightColor.r, rightColor.g, rightColor.b, alpha);

            yield return null;
        }

        // Ensure the sprites are fully transparent at the end
        topSprite.color = new Color(topColor.r, topColor.g, topColor.b, 0f);
        leftSprite.color = new Color(leftColor.r, leftColor.g, leftColor.b, 0f);
        rightSprite.color = new Color(rightColor.r, rightColor.g, rightColor.b, 0f);
    }

    private void SetSpriteAlpha(float alpha)
    {
        Color topColor = topSprite.color;
        Color leftColor = leftSprite.color;
        Color rightColor = rightSprite.color;

        topSprite.color = new Color(topColor.r, topColor.g, topColor.b, alpha);
        leftSprite.color = new Color(leftColor.r, leftColor.g, leftColor.b, alpha);
        rightSprite.color = new Color(rightColor.r, rightColor.g, rightColor.b, alpha);
    }

    private void AlignSpritesWithScreen()
    {
        // Get the screen edges in world coordinates
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));

        // Position the top sprite
        topSprite.transform.position = new Vector3(0, topLeft.y, 0);
        topSprite.size = new Vector2(bottomRight.x * 2, topSprite.size.y); // Adjust width to match screen width

        // Position the left sprite
        leftSprite.transform.position = new Vector3(topLeft.x, 0, 0);
        leftSprite.size = new Vector2(leftSprite.size.x, topLeft.y * 2); // Adjust height to match screen height

        // Position the right sprite
        rightSprite.transform.position = new Vector3(bottomRight.x, 0, 0);
        rightSprite.size = new Vector2(rightSprite.size.x, topLeft.y * 2); // Adjust height to match screen height
    }
}

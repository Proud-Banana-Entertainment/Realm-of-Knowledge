using UnityEngine;

public class QuizCompletionHandler : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioSource completedSfx;

    private Renderer buttonRenderer;
    private bool colorChanged = false;
    private bool isCompleted = false;

    public bool IsCompleted => isCompleted;

    public delegate void QuizCompletedDelegate();
    public event QuizCompletedDelegate OnQuizCompleted;

    private void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
    }

    private void SetButtonColor(Color color)
    {
        buttonRenderer.material.color = color;
    }

    public void QuizCompletion()
    {
        if (!colorChanged)
        {
            completedSfx.Play();
            SetButtonColor(Color.green);
            colorChanged = true;
            isCompleted = true;

            // Raise the OnQuizCompleted event when the quiz is completed
            OnQuizCompleted?.Invoke();
        }
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Canvas")]
    [SerializeField] private GameObject quizCanvas;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Question[] questions;

    [Header("Result Canvas")]
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button exitButton;

    [Header("Score Canvas")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Dependencies")]
    [SerializeField] private FirstPersonController player;

    [SerializeField] private QuizCompletionHandler quizCompletionHandlers;
    [SerializeField] private bool quizCompletion;

    private List<Question> questionPool = new List<Question>();
    private Queue<Question> currentQuestionSet = new Queue<Question>();
    private bool quizTriggered = false;
    private bool quizCompleted = false;
    private int correctAnswerCount = 0;
    private bool quizButtonActivated = false;

    private void Start()
    {
        scoreManager.DisableScoreCanvas();
        quizCanvas.SetActive(false);
        resultCanvas.SetActive(false);
        SetCursorState(false);
        exitButton.onClick.AddListener(ExitQuiz);
        tryAgainButton.onClick.AddListener(TryAgain);
        InitializeQuestionPool();
    }

    private void SetCursorState(bool enabled)
    {
        Cursor.visible = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && quizButtonActivated)
        {
            quizTriggered = true;
            SetCursorState(false);
            scoreManager.ResetScore(); // Reset the score when entering a new room
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizTriggered = false;
            quizCanvas.SetActive(false);
            resultCanvas.SetActive(false);
            SetCursorState(false);
        }
    }

    private void Update()
    {
        if (quizTriggered && !quizCompleted && Input.GetKeyDown(KeyCode.E))
        {
            SetCursorState(true);
            Time.timeScale = 0f;
            player.cameraCanMove = false;
            quizCanvas.SetActive(true);
            StartQuiz();
            scoreManager.EnableScoreCanvas();
        }
    }

    private void StartQuiz()
    {
        if (currentQuestionSet.Count == 0)
        {
            ResetQuiz();
            GenerateQuestionSet();
        }

        DisplayQuestion();
    }

    public void AnswerButtonClicked(int buttonIndex)
    {
        Question currentQuestion = currentQuestionSet.Peek();
        if (buttonIndex == currentQuestion.correctAnswerIndex)
        {
            scoreManager.AddScore(currentQuestion.scorePoints);
            correctAnswerCount++;
            if (correctAnswerCount == 5)
            {
                QuizCompleted();
                return;
            }
        }

        currentQuestionSet.Dequeue();

        if (currentQuestionSet.Count > 0)
        {
            DisplayQuestion();
        }
        else
        {
            QuizCompleted();
        }
    }

    public bool TotalQuizCompleted()
    {
        return quizCompleted;
    }

    private void QuizCompleted()
    {
        Time.timeScale = 0f;
        quizCompleted = true;
        player.cameraCanMove = false;
        quizCanvas.SetActive(false);
        resultCanvas.SetActive(true);
        scoreManager.EnableScoreCanvas();

        string resultMessage = $"{correctAnswerCount} out of 5 answers are correct!";
        resultText.text = resultMessage;

        int currentScore = scoreManager.GetCurrentScore();
        scoreText.text = "Score: " + currentScore;

        if (currentScore == scoreManager.GetRoomScoreLimit() && quizCompletion)
        {
            if (quizCompletionHandlers != null) // Check if quizCompletionHandlers is not null before invoking the method
            {
                quizCompletionHandlers.QuizCompletion();
            }
        }
    }

    private void ExitQuiz()
    {
        SetCursorState(false);
        quizCompleted = false;
        Time.timeScale = 1f;
        player.cameraCanMove = true;
        quizCanvas.SetActive(false);
        resultCanvas.SetActive(false);
        ResetQuiz();
        scoreManager.DisableScoreCanvas();
    }

    private void TryAgain()
    {
        SetCursorState(true);
        Time.timeScale = 0f;
        player.cameraCanMove = false;
        quizCanvas.SetActive(true);
        resultCanvas.SetActive(false);
        scoreManager.ResetScore();
        correctAnswerCount = 0;
        quizCompleted = false;
        ResetQuiz(); // Reset the quiz before starting it again
        StartQuiz();
        scoreManager.EnableScoreCanvas();
    }

    private void InitializeQuestionPool()
    {
        questionPool.AddRange(questions);
    }

    private void GenerateQuestionSet()
    {
        ShuffleQuestions(questionPool);
        foreach (var question in questionPool.GetRange(0, 5))
        {
            currentQuestionSet.Enqueue(question);
        }
    }

    private void DisplayQuestion()
    {
        Question currentQuestion = currentQuestionSet.Peek();
        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button answerButton = answerButtons[i];
            answerButton.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];

            int index = i;
            answerButton.onClick.RemoveAllListeners();
            answerButton.onClick.AddListener(() => AnswerButtonClicked(index));
        }
    }

    private void ResetQuiz()
    {
        correctAnswerCount = 0;
        currentQuestionSet.Clear();
    }

    private void ShuffleQuestions(List<Question> questionList)
    {
        int n = questionList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Question value = questionList[k];
            questionList[k] = questionList[n];
            questionList[n] = value;
        }
    }

    public void ActivateQuizButton()
    {
        quizButtonActivated = true;
    }
}

[System.Serializable]
public class Question
{
    public string question;
    public string[] answers;
    public int correctAnswerIndex;
    public int scorePoints;
}

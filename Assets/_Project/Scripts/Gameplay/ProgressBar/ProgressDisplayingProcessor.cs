using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ProgressDisplayingProcessor
{
    private readonly ProgressBar _progressBar;
    private readonly GlobalCoroutineRunner _coroutineRunner;

    public ProgressDisplayingProcessor(ProgressBar progressBar, GlobalCoroutineRunner coroutineRunner)
    {
        _progressBar = progressBar;
        _coroutineRunner = coroutineRunner;
    }

    private IEnumerator UpdateProgress()
    {
        while (true)
        {
            yield return null;

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnRunnerRunner : Utilr.Singleton<YarnRunnerRunner>
{
    [SerializeField]
    private GameObject m_runnerPrefab = null;
    [SerializeField]
    private InMemoryVariableStorage m_varStor = null;

    private YarnComandeer m_comandeer = null;

    public void Register(string char_id, out DialogueRunner runner, out CustomDialogueUI ui)
    {
        var o = GameObject.Instantiate(m_runnerPrefab, Vector3.zero, Quaternion.identity, transform);
        o.TryGetComponent(out runner);
        o.TryGetComponent(out ui);
        m_comandeer.InitRunner(runner);
        runner.variableStorage = m_varStor;
    }
    private void Awake()
    {
        TryGetComponent(out m_comandeer);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}

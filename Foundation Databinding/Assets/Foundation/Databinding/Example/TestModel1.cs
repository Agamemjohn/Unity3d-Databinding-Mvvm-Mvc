using Foundation.Databinding.Model;

public class TestModel1 : ObservableBehaviour
{
	public bool m_PushString;
	public string m_EditorString;

	public string m_Text;

	public string Text1
	{
		get { return m_Text; }
		set { Set(ref m_Text, value, "Text1"); }
	}

	void Update () 
	{
		// This simulates the model changing through gameplay/etc
		if(m_PushString)
		{
			m_PushString = false;
			Text1 = m_EditorString;
		}
	}
}

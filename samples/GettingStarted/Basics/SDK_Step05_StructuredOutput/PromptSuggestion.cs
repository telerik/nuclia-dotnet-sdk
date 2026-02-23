
using System.ComponentModel;

public class PromptSuggestion
{
	[Description("An array of suggested questions to ask about the knowledge box")]
	public Question[] Questions { get; set; } = [];
}

public class Question
{
	// The description will be used by Progress Agentic RAG to form the response.
	// Think of the description as a hint to the agent about how to use the data in this property when generating a response.
	// Its a small piece of prompt that is prepended to the value of the property when the agent is generating a response.
	// For example, change the text below to "A FOUR word title for the question"
	// and you will see that the agent will generate longer titles for the questions.
	[Description("A three word title for the question")]
	public string Title { get; set; } = "";

	[Description("The full question text that can be used as a search query")]
	public string PromptText { get; set; } = "";
}
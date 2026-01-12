using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using System.Threading.Channels;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Base class for the generated scripts.
    /// </summary>
    /// <param name="project">Project where the script is running.</param>
    /// <param name="scriptDefinition">Script definition for the script.</param>
    public abstract class ScriptBase(RunnableProject project, ScriptDefinition scriptDefinition)
    {
        /// <summary>
        /// The changed instance constant for instance.
        /// </summary>
        public const string ChangedInstance = "ChangedInstance";

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        protected RunnableProject Project { get; } = project;
        /// <summary>
        /// Gets the script definition.
        /// </summary>
        /// <value>
        /// The script definition.
        /// </value>
        protected ScriptDefinition ScriptDefinition { get; } = scriptDefinition;
        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        protected Dictionary<string, IInstance> Instances { get; } = [];

        /// <summary>
        /// Registers the script for topic notification.
        /// </summary>
        /// <returns>The value task.</returns>
        /// <exception cref="SuperVException">Error while processing topic notification.</exception>
        /// <exception cref="UnknownEntityException">Unknown topic</exception>
        public async ValueTask RegisterForTopicNotification()
        {
            if (Project.TopicsChannels.TryGetValue(ScriptDefinition.TopicName, out Channel<FieldValueChangedEvent>? notificationChannel))
            {
                try
                {
                    HashSet<string> instanceReferences = [.. ScriptDefinition.fieldReferences.Select(f => f.InstanceName ?? ChangedInstance)];

                    instanceReferences.Where(instanceName => instanceName != ChangedInstance).ForEach(instanceName =>
                    {
                        IInstance instance = Project.GetInstance(instanceName);
                        Instances.Add(instanceName, instance);
                    });
                    while (await notificationChannel.Reader.WaitToReadAsync())
                    {
                        while (notificationChannel.Reader.TryRead(out FieldValueChangedEvent? fieldValueChangedEvent))
                        {
                            Instances[ChangedInstance] = fieldValueChangedEvent.Field.Instance!;
                            HandleFieldValueChangeInternal(Project, Instances, fieldValueChangedEvent);
                        }
                    }
                    return;
                }
                catch (Exception e)
                {
                    throw new SuperVException("Error while processing topic notification.", e);
                }
            }
            throw new UnknownEntityException("topic", ScriptDefinition.TopicName);
        }

        /// <summary>
        /// Handles the field value change internally. Calls the abstract method HandleFieldValueChange.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="instances">The instances.</param>
        /// <param name="fieldValueChangedEvent">The field value changed event.</param>
        private void HandleFieldValueChangeInternal(RunnableProject project, Dictionary<string, IInstance> instances, FieldValueChangedEvent fieldValueChangedEvent)
        {
            HandleFieldValueChange(project, instances, fieldValueChangedEvent);
        }

        /// <summary>
        /// Handles the field value change. This method must be implemented in the generated class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="instances">The instances.</param>
        /// <param name="fieldValueChangedEvent">The field value changed event.</param>
        public abstract void HandleFieldValueChange(RunnableProject project, Dictionary<string, IInstance> instances, FieldValueChangedEvent fieldValueChangedEvent);
    }
}

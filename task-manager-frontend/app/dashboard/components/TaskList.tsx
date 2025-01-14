interface Task {
  id: number;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
  createdAt: string;
}

interface TaskListProps {
  tasks: Task[];
  onUpdateTask: (taskId: number, updates: Partial<Task>) => void;
  onDeleteTask: (taskId: number) => void;
}

export default function TaskList({
  tasks,
  onUpdateTask,
  onDeleteTask,
}: TaskListProps) {
  if (tasks.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        No tasks found. Create a new task to get started!
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {tasks.map((task) => (
        <div
          key={task.id}
          className="bg-white p-6 rounded-lg shadow-sm border border-gray-200"
        >
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold text-gray-800">
              {task.title}
            </h2>
            <span className="px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
              {task.category}
            </span>
          </div>

          <p className="text-gray-600 mb-4">{task.description}</p>

          <div className="flex items-center justify-between">
            <label className="flex items-center space-x-2">
              <input
                type="checkbox"
                checked={task.isCompleted}
                onChange={(e) =>
                  onUpdateTask(task.id, { isCompleted: e.target.checked })
                }
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
              />
              <span className="text-sm text-gray-600">Completed</span>
            </label>

            <button
              onClick={() => onDeleteTask(task.id)}
              className="text-red-600 hover:text-red-800 focus:outline-none"
            >
              Delete
            </button>
          </div>

          <div className="mt-4 text-xs text-gray-500">
            Created: {new Date(task.createdAt).toLocaleDateString()}
          </div>
        </div>
      ))}
    </div>
  );
}

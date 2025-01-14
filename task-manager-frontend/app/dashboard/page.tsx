"use client";
import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import api from "../../lib/api";
import TaskList from "./components/TaskList";
import CreateTaskModal from "./components/CreateTaskModal";

interface Task {
  id: number;
  title: string;
  description: string;
  category: string;
  isCompleted: boolean;
  createdAt: string;
}

export default function Dashboard() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const router = useRouter();

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        const response = await api.get("/Tasks");
        setTasks(response.data);
      } catch (error) {
        setError("Failed to fetch tasks");
        console.error("Error fetching tasks:", error);
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          router.push("/auth/login");
        }
      } finally {
        setIsLoading(false);
      }
    };

    if (!localStorage.getItem("token")) {
      router.push("/auth/login");
      return;
    }
    fetchTasks();
  }, [router]);

  const handleCreateTask = async (
    newTask: Omit<Task, "id" | "createdAt" | "isCompleted">
  ) => {
    console.log("Creating task with data:", newTask);

    try {
      const response = await api.post("/Tasks", newTask);

      if (response.data) {
        const createdTask = {
          ...response.data,
          isCompleted: false,
          createdAt: new Date().toISOString(),
        };
        setTasks((prevTasks) => [createdTask, ...prevTasks]);
        setIsModalOpen(false);
        setError("");
        console.log("Task created successfully:", createdTask);
      }
    } catch (error) {
      if (axios.isAxiosError(error) && error.response) {
        const errorMessage =
          error.response.data.message ||
          error.response.data.title ||
          "Failed to create task";
        setError(errorMessage);
        console.error("Error response:", error.response.data);
      } else {
        setError("Failed to create task");
      }
    }
  };

  const handleUpdateTask = async (taskId: number, updates: Partial<Task>) => {
    try {
      await api.put(`/Tasks/${taskId}`, updates);
      setTasks(
        tasks.map((task) =>
          task.id === taskId ? { ...task, ...updates } : task
        )
      );
    } catch (error) {
      setError("Failed to update task");
      console.error("Error updating task:", error);
    }
  };

  const handleDeleteTask = async (taskId: number) => {
    try {
      await api.delete(`/Tasks/${taskId}`);
      setTasks(tasks.filter((task) => task.id !== taskId));
    } catch (error) {
      setError("Failed to delete task");
      console.error("Error deleting task:", error);
    }
  };

  return (
    <main className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">My Tasks</h1>
          <button
            onClick={() => setIsModalOpen(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            Create Task
          </button>
        </div>

        {error && (
          <div className="mb-4 p-4 bg-red-50 text-red-700 rounded-md">
            {error}
          </div>
        )}

        {isLoading ? (
          <div className="text-center py-8">Loading tasks...</div>
        ) : (
          <TaskList
            tasks={tasks}
            onUpdateTask={handleUpdateTask}
            onDeleteTask={handleDeleteTask}
          />
        )}

        {isModalOpen && (
          <CreateTaskModal
            onClose={() => setIsModalOpen(false)}
            onCreateTask={handleCreateTask}
          />
        )}
      </div>
    </main>
  );
}

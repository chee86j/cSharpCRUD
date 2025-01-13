import Login from "./login";
import Link from "next/link";

export default function LoginPage() {
  return (
    <div className="min-h-screen bg-gray-100">
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="flex flex-col items-center justify-center space-y-8">
            <div className="text-center space-y-2">
              <h1 className="text-4xl font-bold text-gray-900">Welcome Back</h1>
              <p className="text-gray-600">
                Sign in to access your task management dashboard
              </p>
            </div>

            <Login />

            <div className="text-center space-y-4">
              <p className="text-sm text-gray-600">
                Don&apos;t have an account?{" "}
                <Link
                  href="/auth/register"
                  className="font-medium text-blue-600 hover:text-blue-500"
                >
                  Sign up
                </Link>
              </p>
              <Link
                href="/"
                className="text-sm text-gray-500 hover:text-gray-400"
              >
                Return to Home
              </Link>
            </div>

            {/* Privacy and Terms Links */}
            <div className="text-center text-xs text-gray-500 space-x-4">
              <Link href="/privacy" className="hover:text-gray-700">
                Privacy Policy
              </Link>
              <Link href="/terms" className="hover:text-gray-700">
                Terms of Service
              </Link>
            </div>
          </div>
        </div>
      </div>

      {/* Error Boundary */}
      <div id="error-boundary" className="hidden fixed bottom-0 w-full">
        <div className="max-w-7xl mx-auto px-4 py-3 bg-red-50">
          <div className="flex items-center justify-between">
            <div className="flex items-center">
              <div className="text-red-700">
                <span id="error-message"></span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

// Metadata for the page
export const metadata = {
  title: "Login - Task Manager",
  description: "Sign in to your Task Manager account",
};

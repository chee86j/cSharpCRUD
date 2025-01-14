import Login from "./login";
import Link from "next/link";

export default function LoginPage() {
  return (
    <main className="min-h-screen bg-gradient-to-b from-gray-50 to-gray-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="py-12 sm:py-16">
          <div className="flex flex-col items-center justify-center space-y-8">
            <div className="text-center space-y-3">
              <h1
                className="text-3xl sm:text-4xl font-bold text-gray-900"
                tabIndex={0}
              >
                Welcome Back
              </h1>
              <p
                className="text-base sm:text-lg text-gray-600"
                role="doc-subtitle"
              >
                Sign in to access your task management dashboard
              </p>
            </div>

            <Login />

            <div className="text-center space-y-4">
              <p className="text-sm sm:text-base text-gray-600">
                Don&apos;t have an account?{" "}
                <Link
                  href="/auth/register"
                  className="font-medium text-blue-700 hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 rounded-sm transition-colors"
                >
                  Sign up
                </Link>
              </p>
              <Link
                href="/"
                className="inline-block text-sm sm:text-base text-gray-500 hover:text-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 rounded-sm transition-colors"
              >
                Return to Home
              </Link>
            </div>

            <nav
              className="text-center text-xs sm:text-sm text-gray-500 space-x-6"
              aria-label="Footer"
            >
              <Link
                href="/privacy"
                className="hover:text-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 rounded-sm transition-colors"
              >
                Privacy Policy
              </Link>
              <Link
                href="/terms"
                className="hover:text-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 rounded-sm transition-colors"
              >
                Terms of Service
              </Link>
            </nav>
          </div>
        </div>
      </div>

      {/* Error Boundary */}
      <div
        id="error-boundary"
        role="alert"
        aria-live="polite"
        className="hidden fixed bottom-0 w-full"
      >
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
    </main>
  );
}

// Metadata for the page
export const metadata = {
  title: "Login - Task Manager",
  description: "Sign in to your Task Manager account",
};

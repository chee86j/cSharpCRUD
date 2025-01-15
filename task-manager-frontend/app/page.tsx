"use client";
import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function LandingPage() {
  const [userEmail, setUserEmail] = useState<string>("");
  const router = useRouter();

  useEffect(() => {
    const email = localStorage.getItem("userEmail");
    if (!email) {
      console.log("No user data found, redirecting to login...");
      router.push("/auth/login");
      return;
    }
    console.log("User data fetched successfully:", { email });
    setUserEmail(email);
  }, [router]);

  return (
    <main className="min-h-screen bg-gray-50" role="main">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 sm:py-12">
        <div className="text-center">
          <h1
            className="text-3xl sm:text-4xl font-extrabold text-gray-900 mb-4 sm:mb-8"
            tabIndex={0}
          >
            Welcome, <span className="break-all">{userEmail}</span>! 👋
          </h1>

          <p
            className="text-lg sm:text-xl text-gray-600 mb-6 sm:mb-8"
            tabIndex={0}
          >
            Thank you for logging in to Task Manager
          </p>

          <div className="flex flex-col sm:flex-row items-center justify-center gap-4 sm:gap-6">
            <Link
              href="/dashboard"
              className="w-full sm:w-auto inline-flex items-center justify-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
              aria-label="Go to Dashboard"
            >
              Go to Dashboard
            </Link>

            <Link
              href="/profile"
              className="w-full sm:w-auto inline-flex items-center justify-center px-6 py-3 border border-gray-300 text-base font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 transition-colors"
              aria-label="View your profile"
            >
              View Profile
            </Link>
          </div>

          <button
            onClick={() => {
              localStorage.clear();
              router.push("/auth/login");
            }}
            className="mt-6 text-sm text-gray-500 hover:text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 rounded-md px-4 py-2 transition-colors"
            aria-label="Sign out of your account"
          >
            Sign Out
          </button>
        </div>
      </div>
    </main>
  );
}

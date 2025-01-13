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
      router.push("/auth/login");
      return;
    }
    setUserEmail(email);
  }, [router]);

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="text-center">
          <h1 className="text-4xl font-extrabold text-gray-900 mb-8">
            Welcome, {userEmail}! 👋
          </h1>

          <p className="text-xl text-gray-600 mb-8">
            Thank you for logging in to Task Manager
          </p>

          <div className="space-y-4">
            <div className="inline-flex space-x-4">
              <Link
                href="/dashboard"
                className="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
              >
                Go to Dashboard
              </Link>

              <Link
                href="/profile"
                className="inline-flex items-center px-6 py-3 border border-gray-300 text-base font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
              >
                View Profile
              </Link>
            </div>

            <button
              onClick={() => {
                localStorage.clear();
                router.push("/auth/login");
              }}
              className="text-sm text-gray-500 hover:text-gray-700"
            >
              Sign Out
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

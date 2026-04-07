import type { Stats } from "../types/stats";

export const fetchStats = async (): Promise<Stats> => {
    const response = await fetch("https://localhost:7195/stats");

    if (!response.ok) {
        throw new Error("Failed to fetch stats");
    }

    return response.json();
};
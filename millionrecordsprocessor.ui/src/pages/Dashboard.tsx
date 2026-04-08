import { useEffect, useState } from "react";
import { fetchStats } from "../api/statsApi";
import type { Stats } from "../types/stats";
import { StatCard } from "../components/StatCard";

export const Dashboard = () => {
    const [stats, setStats] = useState<Stats | null>(null);
    const [loading, setLoading] = useState(true);

    const loadData = async () => {
        try {
            const data = await fetchStats();
            setStats(data);
        } catch (error) {
            console.error("Error fetching stats", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();

        const interval = setInterval(loadData, 5000);
        return () => clearInterval(interval);
    }, []);

    if (loading) return <h2>Loading...</h2>;

    return (
        <div style={styles.container}>
            <h1>📊 Processing Dashboard</h1>

            <div style={styles.grid}>
                <StatCard title="Total Records" value={stats?.total ?? 0} />
                <StatCard title="Processed" value={stats?.processed ?? 0} />
                <StatCard title="Failed" value={stats?.failed ?? 0} />
            </div>
        </div>
    );
};

const styles = {
    container: {
        padding: "30px",
        fontFamily: "Arial",
        backgroundColor: "#f5f6fa",
        minHeight: "100vh",
    },
    grid: {
        display: "flex",
        gap: "20px",
        marginTop: "20px",
    },
};



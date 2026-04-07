type Props = {
    title: string;
    value: number;
};

export const StatCard = ({ title, value }: Props) => {
    return (
        <div style={styles.card}>
            <h3>{title}</h3>
            <h1>{value}</h1>
        </div>
    );
};

const styles = {
    card: {
        border: "1px solid #ddd",
        borderRadius: "12px",
        padding: "20px",
        width: "200px",
        textAlign: "center" as const,
        boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
        backgroundColor: "#fff",
    },
};
import { ReactNode } from 'react';
import './detail-card.css';

interface DetailCardProps {
  title: string;
  children: ReactNode;
}

function DetailCard({ title, children }: DetailCardProps) {
  return (
    <div className="detail-card">
      <h3 className="detail-card-title">{title}</h3>
      <div className="detail-card-content">{children}</div>
    </div>
  );
}

export default DetailCard;

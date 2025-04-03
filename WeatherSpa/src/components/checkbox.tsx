import { ChangeEvent } from 'react';
import './checkbox.css';

interface CheckboxProps {
  id: string;
  label: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
  helper?: string;
}

export function Checkbox({ id, label, checked, onChange, helper }: CheckboxProps) {
  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.checked);
  };

  return (
    <div className="checkbox-container">
      <div className="checkbox-wrapper">
        <input
          type="checkbox"
          id={id}
          checked={checked}
          onChange={handleChange}
          className="checkbox-input"
        />
        <label htmlFor={id} className="checkbox-label">
          {label}
        </label>
      </div>
      {helper && <p className="checkbox-helper">{helper}</p>}
    </div>
  );
}

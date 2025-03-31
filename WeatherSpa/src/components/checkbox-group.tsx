import { forwardRef, HTMLAttributes } from 'react';
import './checkbox-group.css';

export interface CheckboxItem {
  id: string;
  label: string;
  description?: string;
}

export type CheckboxGroupProps = Omit<HTMLAttributes<HTMLFieldSetElement>, 'onChange'> & {
  /**
   * Array of checkbox items to display
   */
  items: CheckboxItem[];

  /**
   * Current selected values (object with keys as item ids and boolean values)
   */
  selected: Record<string, boolean>;

  /**
   * Function called when a checkbox is changed
   */
  onChange: (id: string, checked: boolean) => void;

  /**
   * Legend text for the checkbox group
   */
  legend: string;

  /**
   * Helper text for additional context
   */
  helper?: string;

  /**
   * Error message to display
   */
  error?: string;

  /**
   * Number of columns in the grid (for larger screens)
   */
  columns?: 1 | 2 | 3;

  /**
   * Prefix to add to checkbox element IDs
   */
  idPrefix?: string;

  /**
   * Additional CSS class name for the fieldset
   */
  className?: string;
};

/**
 * A group of related checkboxes with labels and descriptions
 */
export const CheckboxGroup = forwardRef<HTMLFieldSetElement, CheckboxGroupProps>(
  (
    {
      items,
      selected,
      onChange,
      legend,
      helper,
      error,
      columns = 1,
      idPrefix = 'cbg',
      className = '',
      ...props
    },
    ref
  ) => {
    const groupId = `${idPrefix}-group`;
    const fieldsetClasses = ['checkbox-group', error ? 'error' : '', className]
      .filter(Boolean)
      .join(' ');

    const gridClasses = [
      'checkbox-group-grid',
      columns === 2 ? 'two-columns' : '',
      columns === 3 ? 'three-columns' : '',
    ]
      .filter(Boolean)
      .join(' ');

    return (
      <fieldset className={fieldsetClasses} ref={ref} {...props}>
        <legend className="checkbox-group-legend">{legend}</legend>

        {helper && <div className="checkbox-group-helper">{helper}</div>}

        <div className={gridClasses} role="group" aria-labelledby={`${groupId}-label`}>
          <div id={`${groupId}-label`} className="ds-visually-hidden">
            {legend}
          </div>

          {items.map(item => (
            <div key={item.id} className="checkbox-item">
              <input
                type="checkbox"
                id={`${idPrefix}-${item.id}`}
                checked={!!selected[item.id]}
                onChange={e => onChange(item.id, e.target.checked)}
                aria-describedby={item.description ? `${idPrefix}-desc-${item.id}` : undefined}
              />
              <label htmlFor={`${idPrefix}-${item.id}`} className="checkbox-label">
                <span className="checkbox-name">{item.label}</span>
                {item.description && (
                  <span id={`${idPrefix}-desc-${item.id}`} className="checkbox-description">
                    {item.description}
                  </span>
                )}
              </label>
            </div>
          ))}
        </div>

        {error && <div className="checkbox-group-error">{error}</div>}
      </fieldset>
    );
  }
);

CheckboxGroup.displayName = 'CheckboxGroup';

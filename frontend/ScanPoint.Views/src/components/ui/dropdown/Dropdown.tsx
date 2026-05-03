import React from "react";

interface DropdownProps {
  isOpen: boolean;
  onClose: () => void;
  className?: string;
  children: React.ReactNode;
}

const Dropdown: React.FC<DropdownProps> = ({ isOpen, onClose, className, children }) => {
  if (!isOpen) return null;

  return (
    <div
      className={className}
      onMouseLeave={onClose} // ose mund të përdorësh click outside logikë
    >
      {children}
    </div>
  );
};

export default Dropdown;

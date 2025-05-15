import React from 'react';

interface SortIconProps {
    active: boolean;
    direction: 'asc' | 'desc';
}

export const SortIcon: React.FC<SortIconProps> = ({ active, direction }) => {
    if (!active) {
        return (
            <span className="ml-1 text-dark-text-secondary">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                     xmlns="http://www.w3.org/2000/svg">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                        d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4"/>
                </svg>
            </span>
        );
    }

    if (direction === 'asc') {
        return (
            <span className="ml-1 text-dark-text-primary">
                 <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7"/>
                  </svg> 
            </span>
        );
    } else {
        return (
            <span className="ml-1 text-dark-text-primary">
                <svg className="w-4 h-4" fill="none"
                     stroke="currentColor" viewBox="0 0 24 24"
                     xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7"/>
            </svg></span>
        );
    }
}

   
          

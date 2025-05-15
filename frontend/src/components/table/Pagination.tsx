import React from 'react';

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
}

export const Pagination: React.FC<PaginationProps> = 
    ({
      currentPage,
      totalPages,
      onPageChange
  }) => {
    if (totalPages <= 1) {
        return null;
    }

    const pageNumbers = [];
    const maxPagesToShow = 5;

    let startPage = Math.max(1, currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage + 1 < maxPagesToShow) {
        startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
        pageNumbers.push(i);
    }

    return (
        <div className="flex justify-center mt-4">
            <nav className="flex items-center ">
                <PaginationButton
                    onClick={() => onPageChange(Math.max(1, currentPage - 1))}
                    disabled={currentPage === 1}
                >
                    Previous
                </PaginationButton>
                {startPage > 1 && (
                    <>
                        <PaginationNumber
                            page={1}
                            currentPage={currentPage}
                            onClick={() => onPageChange(1)}
                        />
                        {startPage > 2 && <span className="px-2">...</span>}
                    </>
                )}

                {pageNumbers.map(number => (
                    <PaginationNumber
                        key={number}
                        page={number}
                        currentPage={currentPage}
                        onClick={() => onPageChange(number)}
                    />
                ))}

                {endPage < totalPages && (
                    <>
                        {endPage < totalPages - 1 && <span className="px-2">...</span>}
                        <PaginationNumber
                            page={totalPages}
                            currentPage={currentPage}
                            onClick={() => onPageChange(totalPages)}
                        />
                    </>
                )}

                <PaginationButton
                    onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
                    disabled={currentPage === totalPages}
                >
                    Next
                </PaginationButton>
            </nav>
        </div>
    );
};

interface PaginationButtonProps {
    onClick: () => void;
    disabled: boolean;
    children: React.ReactNode;
}

const PaginationButton: React.FC<PaginationButtonProps> = 
    ({
       onClick,
       disabled,
       children
   }) => (
    <button
        onClick={onClick}
        disabled={disabled}
        className={`px-3 py-1 hover:cursor-pointer rounded-md ${
            disabled
                ? 'text-dark-text-disabled cursor-not-allowed'
                : 'text-dark-text-primary hover:bg-dark-surface-hover'
        }`}
    >
        {children}
    </button>
);

interface PaginationNumberProps {
    page: number;
    currentPage: number;
    onClick: () => void;
}

const PaginationNumber: React.FC<PaginationNumberProps> = 
        ({
           page,
           currentPage,
           onClick
       }) => (
    <button
        onClick={onClick}
        className={`w-8 h-8 flex hover:cursor-pointer items-center justify-center rounded-md ${
            page === currentPage
                ? 'bg-blue-500 text-white'
                : 'text-dark-text-primary hover:bg-dark-surface-hover'
        }`}
    >
        {page}
    </button>
);
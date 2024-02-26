import React from "react";
import { Link } from "react-router-dom";

interface PaginationProps {
  totalPages: number;
  currentPage: number;
  onPageChange: (page: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({
  totalPages,
  currentPage,
  onPageChange,
}) => {
  const pages = [];

  if (currentPage > 4) {
    pages.push(
      <li key={1} className="page-item">
        <Link className="page-link" to={`?page=1`} onClick={() => onPageChange(1)}>
          1
        </Link>
      </li>
    );
    if (currentPage > 5) {
      pages.push(<li key="left-ellipsis" className="page-item disabled"><span className="page-link">...</span></li>);
    }
  }

  for (
    let p = Math.max(currentPage - 3, 1);
    p <= Math.min(currentPage + 3, totalPages);
    p++
  ) {
    pages.push(
      <li key={p} className={`page-item${currentPage === p ? " active" : ""}`}>
        <Link
          className="page-link"
          to={`?page=${p}`}
          onClick={() => onPageChange(p)}
        >
          {p}
        </Link>
      </li>
    );
  }

  if (currentPage < totalPages - 3) {
    if (currentPage < totalPages - 4) {
      pages.push(<li key="right-ellipsis" className="page-item disabled"><span className="page-link">...</span></li>);
    }
    pages.push(
      <li key={totalPages} className="page-item">
        <Link
          className="page-link"
          to={`?page=${totalPages}`}
          onClick={() => onPageChange(totalPages)}
        >
          {totalPages}
        </Link>
      </li>
    );
  }

  return (
    <nav aria-label="Pagination">
      <ul className="pagination">
        {pages}
      </ul>
    </nav>
  );
};

export default Pagination;

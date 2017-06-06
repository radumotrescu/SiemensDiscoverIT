# -*- coding: utf-8 -*-
"""
Created on Tue Jun  6 15:26:00 2017

@author: Radu.Motrescu
"""

from scipy.spatial.distance import pdist, squareform
from scipy import exp
from scipy.linalg import eigh
import numpy as np


def stepwise_kpca(X, gamma, n_components):
    sq_dist=pdist(X, 'sqeuclidian')
    
    mat_sq_dists=squareform(sq_dist)
    
    K=exp(-gamma * mat_sq_dists)
    
    N=K.shape[0]
    one_n=np.ones((N,N))/N
    K=K-one_n.dot(K) - K.dot(one_n) + one_n.dot(K).dot(one_n)
    
    eigvals, eigvecs=eigh(K)
    
    X_pc=np.column_stack((eigvecs[:,-i] for i in range (1,n_components+1)))
    
    return X_pc


count = length(origImages);

maxI=0;
sumI=0;
for I=1:count
    maxV= max( max(origImages{I}));
    sumI=sumI+maxV;
    maxI=max(maxI,maxV);
end

aveMax=sumI/count;

convertI =255/( aveMax*1.2);

testImages=cell(size(origImages));
for I=1:count
    testImages{I}=origImages{I}*(convertI);
end


psf=[];
tpsf=[];
figure;
for I=1:count
    
    I1=I
    I2=mod( I,count)+1;
    I3=mod( I+250-1,count)+1;
    I4=mod( I+251-1,count)+1;
    
    h0 = testImages{I1};
    h1 = testImages{I2};
    h2 = flipud( testImages{I3});
    h3 = flipud( testImages{I4});

  % Poisson scaling factor
    alpha = 1;
    
    % Gaussian component N(g,sigma^2)
    sigma =25.0/255;
    g = 0.0;
    
    fz0 = GenAnscombe_forward(h0,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
    fz2 = GenAnscombe_forward(h2,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
     
    y={fz0 fz2 };


    psf=[];
    [image ] = DeconvolveImageOBD(y,true, 1.0, 28);
  
  yhat = GenAnscombe_inverse_exact_unbiased(image,sigma,alpha,g);   % exact unbiased inverse
%  imagesc(h0+h1+h2+h3);
%  colormap gray;
%  drawnow;
    cleanedImagesPN{I}=yhat;
    
end


for I=1:count
    
    I1=I
    I2=mod( I,count)+1;
    I3=mod( I+250-1,count)+1;
    I4=mod( I+251-1,count)+1;
    
    h0 = testImages{I1};
    h1 = testImages{I2};
    h2 = flipud( testImages{I3});
    h3 = flipud( testImages{I4});

 
    fz0 = GenAnscombe_forward(h0,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
    fz2 = GenAnscombe_forward(h2,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
     
    y={fz0 fz2 };


    psf=[];
    [image ] = DeconvolveImageOBD(y,true, 2.0,250);
  
    yhat = GenAnscombe_inverse_exact_unbiased(image,sigma,alpha,g);   % exact unbiased inverse
    cleanedImagesPN2{I}=yhat;
   
end
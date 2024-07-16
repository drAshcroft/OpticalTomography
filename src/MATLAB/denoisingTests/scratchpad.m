
testImages=origImages;

nProjections =length(testImages)

step=(length(testImages))/nProjections;

angleStep=360.0/(nProjections+1);

t=testImages{2};
%t= imresize(t,.5);
s = size(t);


sinoGram = zeros([s(2) nProjections]);
dnImages = zeros([nProjections s(1) s(2)]);
figure;

for I=1:nProjections
    projection =testImages{I};
    
%     fz = GenAnscombe_forward(projection,sigma,alpha,g);D= wiener2(fz,[250 250],100);  yhat = GenAnscombe_inverse_exact_unbiased(D,sigma,alpha,g);
%     projection=yhat;
    
   % projection = imresize(projection,.5);
   % projection = doBM3DImageFilter(projection);
   % cleanedImagesP2BM{I}=projection;
    %imagesc(projection(100:end-100,100:end-100));drawnow;
    projection=medfilt2(projection,[5 5]);
    imagesc(projection);drawnow;
    colormap gray;
    dnImages(I,:,:)=projection;
    projection = projection(:,round(s(1)/2));
    Projections{I}=projection';
    sinoGram(:,I)=projection(:);
end

angleStep=360.0/(nProjections+1);
thetas = ((1:nProjections)-1)*angleStep;

reconImage=iradon(sinoGram,thetas,'linear','Hann',1,s(2));

%figure;imagesc(reconImage(70:end-70,70:end-70));colormap gray;
D=reconImage;
figure;imagesc(reconImage);colormap gray;